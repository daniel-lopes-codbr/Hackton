using AgroSolutions.Domain.Entities;
using AgroSolutions.Functions.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AgroSolutions.Functions.Tests.Services;

public class DataProcessingServiceTests
{
    private readonly DataProcessingService _service;
    private readonly ILogger<DataProcessingService> _logger;
    private readonly IAnalyticsService _analyticsService;

    public DataProcessingServiceTests()
    {
        _logger = new LoggerFactory().CreateLogger<DataProcessingService>();
        _analyticsService = new AnalyticsService(new LoggerFactory().CreateLogger<AnalyticsService>());
        _service = new DataProcessingService(_logger, _analyticsService);
    }

    [Fact]
    public async Task ProcessReadingAsync_Should_Process_Normal_Reading()
    {
        // Arrange
        var reading = new SensorReading(
            Guid.NewGuid(),
            "Temperature",
            25.5m,
            "Celsius",
            DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessReadingAsync(reading);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reading, result.OriginalReading);
        Assert.False(result.IsAnomaly);
        Assert.NotNull(result.NormalizedValue);
        Assert.NotNull(result.Insights);
    }

    [Fact]
    public async Task ProcessReadingAsync_Should_Detect_Anomaly_When_Value_Too_High()
    {
        // Arrange
        var reading = new SensorReading(
            Guid.NewGuid(),
            "Temperature",
            55m, // Above threshold of 50
            "Celsius",
            DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessReadingAsync(reading);

        // Assert
        Assert.True(result.IsAnomaly);
        Assert.NotNull(result.AnomalyReason);
        Assert.Contains("above maximum threshold", result.AnomalyReason);
    }

    [Fact]
    public async Task ProcessReadingAsync_Should_Detect_Anomaly_When_Value_Too_Low()
    {
        // Arrange
        var reading = new SensorReading(
            Guid.NewGuid(),
            "Temperature",
            -5m, // Below threshold of 0
            "Celsius",
            DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessReadingAsync(reading);

        // Assert
        Assert.True(result.IsAnomaly);
        Assert.NotNull(result.AnomalyReason);
        Assert.Contains("below minimum threshold", result.AnomalyReason);
    }

    [Fact]
    public async Task ProcessReadingAsync_Should_Normalize_Fahrenheit_To_Celsius()
    {
        // Arrange
        var reading = new SensorReading(
            Guid.NewGuid(),
            "Temperature",
            77m, // 77°F = 25°C
            "Fahrenheit",
            DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessReadingAsync(reading);

        // Assert
        Assert.NotNull(result.NormalizedValue);
        Assert.InRange(result.NormalizedValue.Value, 24m, 26m); // Approximately 25°C
    }

    [Fact]
    public async Task ProcessBatchAsync_Should_Process_All_Readings()
    {
        // Arrange
        var readings = new List<SensorReading>
        {
            new(Guid.NewGuid(), "Temperature", 25m, "Celsius", DateTime.UtcNow),
            new(Guid.NewGuid(), "Humidity", 60m, "Percent", DateTime.UtcNow),
            new(Guid.NewGuid(), "SoilMoisture", 45m, "Percent", DateTime.UtcNow)
        };

        // Act
        var results = await _service.ProcessBatchAsync(readings);

        // Assert
        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.NotNull(r));
    }

    [Fact]
    public async Task ProcessReadingAsync_Should_Generate_Recommendations()
    {
        // Arrange
        var reading = new SensorReading(
            Guid.NewGuid(),
            "Temperature",
            40m, // High temperature
            "Celsius",
            DateTime.UtcNow
        );

        // Act
        var result = await _service.ProcessReadingAsync(reading);

        // Assert
        Assert.NotNull(result.Insights);
        Assert.True(result.Insights.ContainsKey("recommendations"));
    }
}

