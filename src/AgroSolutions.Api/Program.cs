using AgroSolutions.Api.Services;
using AgroSolutions.Api.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/agrosolutions-.log", rollingInterval: RollingInterval.Day)
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
            Title = "AgroSolutions API - FASE 4: Observability & Final Delivery",
            Version = "v1",
            Description = "API for high-performance ingestion of agricultural sensor data with full observability",
            Contact = new OpenApiContact
            {
                Name = "AgroSolutions Team",
                Email = "support@agrosolutions.com"
            }
        });
    });

    // Register services
    builder.Services.AddScoped<IIngestionService, IngestionService>();

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
