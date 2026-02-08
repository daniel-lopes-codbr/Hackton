namespace AgroSolutions.Functions.Services;

/// <summary>
/// Service for analytics and trend analysis
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Gets trend analysis for a field and sensor type
    /// </summary>
    Task<TrendAnalysis?> GetTrendAsync(Guid fieldId, string sensorType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for a field and sensor type
    /// </summary>
    Task<SensorStatistics?> GetStatisticsAsync(Guid fieldId, string sensorType, CancellationToken cancellationToken = default);
}

/// <summary>
/// Trend analysis result
/// </summary>
public class TrendAnalysis
{
    public string Trend { get; set; } = string.Empty; // "Increasing", "Decreasing", "Stable"
    public decimal? ChangeRate { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Sensor statistics
/// </summary>
public class SensorStatistics
{
    public decimal? Average { get; set; }
    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
    public int ReadingCount { get; set; }
}

