using AgroSolutions.Domain.Entities;
using AgroSolutions.Functions.Services;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Functions.Services;

/// <summary>
/// Service that processes sensor data with intelligence and anomaly detection
/// </summary>
public class DataProcessingService : IDataProcessingService
{
    private readonly ILogger<DataProcessingService> _logger;
    private readonly IAnalyticsService _analyticsService;

    // Thresholds for anomaly detection
    private readonly Dictionary<string, (decimal Min, decimal Max)> _sensorThresholds = new()
    {
        { "Temperature", (0m, 50m) },
        { "Humidity", (0m, 100m) },
        { "SoilMoisture", (0m, 100m) },
        { "pH", (4m, 9m) }
    };

    public DataProcessingService(ILogger<DataProcessingService> logger, IAnalyticsService analyticsService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
    }

    public async Task<ProcessedReading> ProcessReadingAsync(SensorReading reading, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing reading: {SensorType} = {Value} {Unit} for Field {FieldId}",
            reading.SensorType, reading.Value, reading.Unit, reading.FieldId);

        var processed = new ProcessedReading
        {
            OriginalReading = reading,
            ProcessedAt = DateTime.UtcNow
        };

        // Normalize value based on sensor type
        processed.NormalizedValue = NormalizeValue(reading.SensorType, reading.Value, reading.Unit);

        // Detect anomalies
        var anomalyResult = DetectAnomaly(reading);
        processed.IsAnomaly = anomalyResult.IsAnomaly;
        processed.AnomalyReason = anomalyResult.Reason;

        // Generate insights
        processed.Insights = await GenerateInsightsAsync(reading, cancellationToken);

        _logger.LogInformation("Processed reading: Anomaly={IsAnomaly}, Insights={InsightCount}",
            processed.IsAnomaly, processed.Insights?.Count ?? 0);

        return processed;
    }

    public async Task<List<ProcessedReading>> ProcessBatchAsync(IEnumerable<SensorReading> readings, CancellationToken cancellationToken = default)
    {
        var readingsList = readings.ToList();
        _logger.LogInformation("Processing batch of {Count} readings", readingsList.Count);

        var tasks = readingsList.Select(reading => ProcessReadingAsync(reading, cancellationToken));
        var results = await Task.WhenAll(tasks);

        _logger.LogInformation("Batch processing completed: {TotalCount} processed, {AnomalyCount} anomalies detected",
            results.Length, results.Count(r => r.IsAnomaly));

        return results.ToList();
    }

    private decimal? NormalizeValue(string sensorType, decimal value, string unit)
    {
        // Normalize to standard units for comparison
        return sensorType.ToLowerInvariant() switch
        {
            "temperature" when unit.Equals("Fahrenheit", StringComparison.OrdinalIgnoreCase) => (value - 32) * 5 / 9, // Convert to Celsius
            "temperature" when unit.Equals("Celsius", StringComparison.OrdinalIgnoreCase) => value,
            "humidity" or "soilmoisture" when unit.Equals("Percent", StringComparison.OrdinalIgnoreCase) => value,
            _ => value // Keep original if unknown
        };
    }

    private (bool IsAnomaly, string? Reason) DetectAnomaly(SensorReading reading)
    {
        if (!_sensorThresholds.TryGetValue(reading.SensorType, out var threshold))
        {
            return (false, null); // Unknown sensor type, no anomaly detection
        }

        var normalizedValue = NormalizeValue(reading.SensorType, reading.Value, reading.Unit) ?? reading.Value;

        if (normalizedValue < threshold.Min)
        {
            return (true, $"Value {normalizedValue} is below minimum threshold {threshold.Min}");
        }

        if (normalizedValue > threshold.Max)
        {
            return (true, $"Value {normalizedValue} is above maximum threshold {threshold.Max}");
        }

        return (false, null);
    }

    private async Task<Dictionary<string, object>> GenerateInsightsAsync(SensorReading reading, CancellationToken cancellationToken)
    {
        var insights = new Dictionary<string, object>();

        // Get trend analysis
        var trend = await _analyticsService.GetTrendAsync(reading.FieldId, reading.SensorType, cancellationToken);
        if (trend != null)
        {
            insights["trend"] = trend;
        }

        // Calculate statistics
        var stats = await _analyticsService.GetStatisticsAsync(reading.FieldId, reading.SensorType, cancellationToken);
        if (stats != null)
        {
            insights["statistics"] = stats;
        }

        // Add recommendations based on sensor type
        var recommendations = GetRecommendations(reading);
        if (recommendations.Any())
        {
            insights["recommendations"] = recommendations;
        }

        return insights;
    }

    private List<string> GetRecommendations(SensorReading reading)
    {
        var recommendations = new List<string>();
        var normalizedValue = NormalizeValue(reading.SensorType, reading.Value, reading.Unit) ?? reading.Value;

        switch (reading.SensorType.ToLowerInvariant())
        {
            case "temperature":
                if (normalizedValue > 35)
                    recommendations.Add("High temperature detected. Consider irrigation or shading.");
                else if (normalizedValue < 10)
                    recommendations.Add("Low temperature detected. Consider protective measures.");
                break;

            case "humidity":
                if (normalizedValue < 30)
                    recommendations.Add("Low humidity detected. Consider increasing irrigation.");
                else if (normalizedValue > 80)
                    recommendations.Add("High humidity detected. Monitor for fungal diseases.");
                break;

            case "soilmoisture":
                if (normalizedValue < 30)
                    recommendations.Add("Low soil moisture. Irrigation recommended.");
                else if (normalizedValue > 80)
                    recommendations.Add("High soil moisture. Risk of root rot.");
                break;
        }

        return recommendations;
    }
}
