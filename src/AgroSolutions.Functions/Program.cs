using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AgroSolutions.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Register services
        services.AddScoped<IDataProcessingService, DataProcessingService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
    })
    .Build();

host.Run();
