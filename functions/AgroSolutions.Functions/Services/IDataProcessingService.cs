using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Functions.Services;

/// <summary>
/// Service for processing sensor data with intelligence
/// </summary>
public interface IDataProcessingService
{
    /// <summary>
    /// Processes a sensor reading and applies intelligence/analysis
    /// </summary>
    Task<ProcessedReading> ProcessReadingAsync(SensorReading reading, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes multiple readings in batch
    /// </summary>
    Task<List<ProcessedReading>> ProcessBatchAsync(IEnumerable<SensorReading> readings, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of processing a sensor reading with intelligence
/// </summary>
public class ProcessedReading
{
    public SensorReading OriginalReading { get; set; } = null!;
    public bool IsAnomaly { get; set; }
    public string? AnomalyReason { get; set; }
    public decimal? NormalizedValue { get; set; }
    public Dictionary<string, object>? Insights { get; set; }
    public DateTime ProcessedAt { get; set; }
}

