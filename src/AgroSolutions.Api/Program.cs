using AgroSolutions.Application.Services;
using AgroSolutions.Api.HealthChecks;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Infrastructure.Data;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Infrastructure.Data.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IO;
using System.Security.Claims;
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

        // Add JWT Bearer authentication to Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: \"Bearer 12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Configure Database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=agrosolutions;Username=postgres;Password=postgres";

    builder.Services.AddDbContext<AgroSolutionsDbContext>(options =>
    {
        // Development: prefer PostgreSQL, fall back to InMemory or SQLite if explicitly configured
        if (builder.Environment.IsDevelopment())
        {
            // In-memory DB for tests/dev scenarios
            if (connectionString.Contains(":memory:"))
            {
                options.UseInMemoryDatabase("AgroSolutionsDb");
            }
            // Detect Postgres-style connection strings
            else if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
                  || connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase)
                  || connectionString.Contains("Port=", StringComparison.OrdinalIgnoreCase))
            {
                // Use Npgsql provider for PostgreSQL
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly("AgroSolutions.Api"));
            }
            // Keep a fallback to SQLite if explicitly provided (backwards compatibility)
            else if (connectionString.Contains("Data Source=") || connectionString.EndsWith(".db"))
            {
                options.UseSqlite(connectionString, b => b.MigrationsAssembly("AgroSolutions.Api"));
            }
            else
            {
                // Default to PostgreSQL in development
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly("AgroSolutions.Api"));
            }
        }
        else
        {
            // Production: prefer PostgreSQL if connection string indicates it, otherwise keep SQL Server
            if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
                || connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase)
                || connectionString.Contains("Port=", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly("AgroSolutions.Api"));
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

    // Register Unit of Work
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Register repositories
    builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
    builder.Services.AddScoped<IFarmRepository, FarmRepository>();
    builder.Services.AddScoped<IFieldRepository, FieldRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IAlertRepository, AlertRepository>();

    // Register services
    builder.Services.AddScoped<IIngestionService, IngestionService>();
    builder.Services.AddScoped<IFarmService, FarmService>();
    builder.Services.AddScoped<IFieldService, FieldService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IAlertService, AlertService>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = ClaimTypes.Role, // Explicitly set the role claim type
            NameClaimType = ClaimTypes.NameIdentifier // Explicitly set the name claim type
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
    });

    // Add Health Checks (general = API + database; ingestion = ingestion service)
    builder.Services.AddHealthChecks()
        .AddCheck<GeneralHealthCheck>("general", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" })
        .AddCheck<IngestionHealthCheck>("ingestion_service", failureStatus: HealthStatus.Degraded, tags: new[] { "ready" });

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

    // Allow skipping DB initialization during design-time (EF tools) by setting SKIP_DB_INIT=1
    var skipDbInit = string.Equals(Environment.GetEnvironmentVariable("SKIP_DB_INIT"), "1", StringComparison.OrdinalIgnoreCase);
    
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

    // Ensure database is created and seeded (for SQLite, InMemory, or SQL Server)
    if (!skipDbInit)
    {
        using (var scope = app.Services.CreateScope())
        {
        var context = scope.ServiceProvider.GetRequiredService<AgroSolutionsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("=== Database Setup Starting ===");
            logger.LogInformation("Connection String: {ConnectionString}", connectionString);
            logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
            
            if (app.Environment.IsDevelopment())
            {
                logger.LogInformation("Development DB initialization. Provider: {Provider}", context.Database.ProviderName);
                try
                {
                    if (context.Database.IsRelational())
                    {
                        logger.LogInformation("Applying migrations...");
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Migrations applied successfully");
                    }
                    else
                    {
                        logger.LogInformation("Ensuring non-relational DB is created...");
                        await context.Database.EnsureCreatedAsync();
                    }
                }
                catch (Exception dbEx)
                {
                    logger.LogError(dbEx, "✗ Error initializing development database: {ErrorMessage}", dbEx.Message);
                }
            }
            else
            {
                // For production: apply migrations for relational providers
                logger.LogInformation("Ensuring production database is ready. Provider: {Provider}", context.Database.ProviderName);
                try
                {
                    if (context.Database.IsRelational())
                    {
                        await context.Database.MigrateAsync();
                    }
                    else
                    {
                        await context.Database.EnsureCreatedAsync();
                    }
                }
                catch (Exception prodEx)
                {
                    logger.LogError(prodEx, "✗ Error initializing production database: {ErrorMessage}", prodEx.Message);
                }
            }

            // Seed initial admin user if database is empty
            logger.LogInformation("=== Starting Database Seeding ===");
            var seeder = new DatabaseSeeder(context, scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>());
            await seeder.SeedAsync();
            logger.LogInformation("=== Database Seeding Completed ===");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "✗ Error during database setup: {ErrorMessage}\nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
            // Don't throw - allow application to start even if database setup fails
        }
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

// Make Program class accessible for integration tests
public partial class Program { }
