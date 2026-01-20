using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AgroSolutions.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Application Insights can be added here if needed
        // services.AddApplicationInsightsTelemetryWorkerService();
        // services.ConfigureFunctionsApplicationInsights();
        
        // Register services
        services.AddScoped<IDataProcessingService, DataProcessingService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
    })
    .Build();

host.Run();
