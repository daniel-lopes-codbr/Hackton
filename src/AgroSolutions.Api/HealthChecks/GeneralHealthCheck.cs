using Microsoft.Extensions.Diagnostics.HealthChecks;
using AgroSolutions.Infrastructure.Data;

namespace AgroSolutions.Api.HealthChecks;

/// <summary>
/// General health check for the API (database connectivity and application readiness).
/// </summary>
public class GeneralHealthCheck : IHealthCheck
{
    private readonly AgroSolutionsDbContext _dbContext;
    private readonly ILogger<GeneralHealthCheck> _logger;

    public GeneralHealthCheck(AgroSolutionsDbContext dbContext, ILogger<GeneralHealthCheck> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                _logger.LogWarning("General health check failed: database is not reachable");
                return HealthCheckResult.Unhealthy(
                    "Database is not reachable",
                    data: new Dictionary<string, object>
                    {
                        { "timestamp", DateTime.UtcNow }
                    });
            }

            return HealthCheckResult.Healthy(
                "API and database are operational",
                new Dictionary<string, object>
                {
                    { "database", "connected" },
                    { "timestamp", DateTime.UtcNow }
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "General health check failed");
            return HealthCheckResult.Unhealthy(
                "API or database is not operational",
                ex,
                new Dictionary<string, object>
                {
                    { "timestamp", DateTime.UtcNow }
                });
        }
    }
}
