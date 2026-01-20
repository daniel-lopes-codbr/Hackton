using AgroSolutions.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Functions.Services;

/// <summary>
/// Service for analytics and trend analysis of sensor data
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly Dictionary<Guid, List<SensorReading>> _fieldReadings = new(); // In-memory store for FASE 3

    public AnalyticsService(ILogger<AnalyticsService> logger)
    {
        _logger = logger;
    }

    public Task<TrendAnalysis?> GetTrendAsync(Guid fieldId, string sensorType, CancellationToken cancellationToken = default)
    {
        if (!_fieldReadings.TryGetValue(fieldId, out var readings))
        {
            return Task.FromResult<TrendAnalysis?>(null);
        }

        var sensorReadings = readings
            .Where(r => r.SensorType.Equals(sensorType, StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.ReadingTimestamp)
            .ToList();

        if (sensorReadings.Count < 2)
        {
            return Task.FromResult<TrendAnalysis?>(null);
        }

        var recent = sensorReadings.TakeLast(10).ToList();
        var older = sensorReadings.Skip(Math.Max(0, sensorReadings.Count - 20)).Take(10).ToList();

        if (older.Count == 0 || recent.Count == 0)
        {
            return Task.FromResult<TrendAnalysis?>(null);
        }

        var olderAvg = older.Average(r => r.Value);
        var recentAvg = recent.Average(r => r.Value);
        var changeRate = ((recentAvg - olderAvg) / olderAvg) * 100;

        var trend = changeRate switch
        {
            > 5 => "Increasing",
            < -5 => "Decreasing",
            _ => "Stable"
        };

        var analysis = new TrendAnalysis
        {
            Trend = trend,
            ChangeRate = changeRate,
            Description = $"Average {sensorType} changed by {changeRate:F2}% over recent readings"
        };

        _logger.LogInformation("Trend analysis for Field {FieldId}, Sensor {SensorType}: {Trend} ({ChangeRate:F2}%)",
            fieldId, sensorType, trend, changeRate);

        return Task.FromResult<TrendAnalysis?>(analysis);
    }

    public Task<SensorStatistics?> GetStatisticsAsync(Guid fieldId, string sensorType, CancellationToken cancellationToken = default)
    {
        if (!_fieldReadings.TryGetValue(fieldId, out var readings))
        {
            return Task.FromResult<SensorStatistics?>(null);
        }

        var sensorReadings = readings
            .Where(r => r.SensorType.Equals(sensorType, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (sensorReadings.Count == 0)
        {
            return Task.FromResult<SensorStatistics?>(null);
        }

        var stats = new SensorStatistics
        {
            Average = sensorReadings.Average(r => r.Value),
            Min = sensorReadings.Min(r => r.Value),
            Max = sensorReadings.Max(r => r.Value),
            ReadingCount = sensorReadings.Count
        };

        _logger.LogInformation("Statistics for Field {FieldId}, Sensor {SensorType}: Avg={Average}, Min={Min}, Max={Max}, Count={Count}",
            fieldId, sensorType, stats.Average, stats.Min, stats.Max, stats.ReadingCount);

        return Task.FromResult<SensorStatistics?>(stats);
    }

    // Helper method to add readings (called by functions)
    public void AddReading(SensorReading reading)
    {
        if (!_fieldReadings.ContainsKey(reading.FieldId))
        {
            _fieldReadings[reading.FieldId] = new List<SensorReading>();
        }

        _fieldReadings[reading.FieldId].Add(reading);

        // Keep only last 1000 readings per field to prevent memory issues
        if (_fieldReadings[reading.FieldId].Count > 1000)
        {
            _fieldReadings[reading.FieldId] = _fieldReadings[reading.FieldId]
                .OrderByDescending(r => r.ReadingTimestamp)
                .Take(1000)
                .ToList();
        }
    }
}
