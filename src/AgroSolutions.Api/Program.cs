using AgroSolutions.Application.Services;
using AgroSolutions.Api.HealthChecks;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Domain.Data;
using AgroSolutions.Domain.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using HealthChecks.UI.Client;
using HealthChecks.UI;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MediatR;
using FluentValidation;
using System.Reflection;
using AutoMapper;

// Configure Serilog
// Ensure logs directory exists
var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logsDirectory))
{
    Directory.CreateDirectory(logsDirectory);
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logsDirectory, "agrosolutions-.log"), rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();

try
{
    Log.Information("Starting AgroSolutions API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AgroSolutions API - FASE 5: Data Persistence",
            Version = "v1",
            Description = "API for high-performance ingestion of agricultural sensor data with full observability",
            Contact = new OpenApiContact
            {
                Name = "AgroSolutions Team",
                Email = "support@agrosolutions.com"
            }
        });
    });

    // Configure Database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=localhost;Database=AgroSolutions;Trusted_Connection=true;TrustServerCertificate=true;";
    
    builder.Services.AddDbContext<AgroSolutionsDbContext>(options =>
    {
        // Use InMemory for development, SQL Server for production
        if (builder.Environment.IsDevelopment() || string.IsNullOrEmpty(connectionString) || connectionString.Contains(":memory:"))
        {
            options.UseInMemoryDatabase("AgroSolutionsDb");
        }
        else
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("AgroSolutions.Api");
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        }
    });

    // Register MediatR (from Application layer)
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AgroSolutions.Application.Services.IUserService).Assembly));

    // Register FluentValidation (from Application layer)
    builder.Services.AddValidatorsFromAssembly(typeof(AgroSolutions.Application.Services.IUserService).Assembly);

    // Register AutoMapper (from Application layer)
    builder.Services.AddAutoMapper(typeof(AgroSolutions.Application.Services.IUserService).Assembly);

    // Register NotificationContext (scoped per request)
    builder.Services.AddScoped<NotificationContext>();

    // Register repositories
    builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
    builder.Services.AddScoped<IFarmRepository, FarmRepository>();
    builder.Services.AddScoped<IFieldRepository, FieldRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    // Register services
    builder.Services.AddScoped<IIngestionService, IngestionService>();
    builder.Services.AddScoped<IFarmService, FarmService>();
    builder.Services.AddScoped<IFieldService, FieldService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // Configure JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeInConfigurationAndAtLeast32CharactersLong";
    var issuer = jwtSettings["Issuer"] ?? "AgroSolutions";
    var audience = jwtSettings["Audience"] ?? "AgroSolutions";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
    });

    // Add Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy("API is healthy"))
        .AddCheck<IngestionHealthCheck>("ingestion_service");

    // Add Health Checks UI
    builder.Services.AddHealthChecksUI(setup =>
    {
        setup.SetEvaluationTimeInSeconds(10);
        setup.MaximumHistoryEntriesPerEndpoint(50);
    }).AddInMemoryStorage();

    // Configure for high performance
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = null; // Keep original property names
    });

    // Configure Kestrel for high performance
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.MaxConcurrentConnections = 1000;
        options.Limits.MaxConcurrentUpgradedConnections = 1000;
        options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB for batch requests
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgroSolutions API v1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at root
        });
    }

    // Only use HTTPS redirection if HTTPS is enabled
    if (app.Configuration.GetValue<bool>("UseHttps", false))
    {
        app.UseHttpsRedirection();
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Map Health Checks
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false
    });

    // Health Checks UI
    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/health-ui";
        options.ApiPath = "/health-ui-api";
    });

    // Ensure database is created (for InMemory or development)
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AgroSolutionsDbContext>();
        if (app.Environment.IsDevelopment())
        {
            // InMemory database is created automatically
            // For SQL Server, uncomment the line below to apply migrations
            // context.Database.Migrate();
        }
    }

    Log.Information("AgroSolutions API started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
