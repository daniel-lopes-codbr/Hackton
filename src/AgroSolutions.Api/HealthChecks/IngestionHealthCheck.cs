using Microsoft.Extensions.Diagnostics.HealthChecks;

using AgroSolutions.Api.Services;

namespace AgroSolutions.Api.HealthChecks;

/// <summary>
/// Health check for ingestion service
/// </summary>
public class IngestionHealthCheck : IHealthCheck
{
    private readonly IIngestionService _ingestionService;
    private readonly ILogger<IngestionHealthCheck> _logger;

    public IngestionHealthCheck(IIngestionService ingestionService, ILogger<IngestionHealthCheck> logger)
    {
        _ingestionService = ingestionService;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple health check - verify service is available
            var readings = _ingestionService.GetAllReadings();
            
            return Task.FromResult(HealthCheckResult.Healthy(
                "Ingestion service is operational",
                new Dictionary<string, object>
                {
                    { "total_readings", readings.Count() },
                    { "timestamp", DateTime.UtcNow }
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for ingestion service");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Ingestion service is not operational",
                ex));
        }
    }
}
