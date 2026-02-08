using AgroSolutions.Domain.Entities;
using AgroSolutions.Functions.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AgroSolutions.Functions.Tests.Services;

public class AnalyticsServiceTests
{
    private readonly AnalyticsService _service;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsServiceTests()
    {
        _logger = new LoggerFactory().CreateLogger<AnalyticsService>();
        _service = new AnalyticsService(_logger);
    }

    [Fact]
    public async Task GetTrendAsync_Should_Return_Null_For_No_Readings()
    {
        // Arrange
        var fieldId = Guid.NewGuid();

        // Act
        var result = await _service.GetTrendAsync(fieldId, "Temperature");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTrendAsync_Should_Detect_Increasing_Trend()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var baseTime = DateTime.UtcNow.AddHours(-2);

        // Add older readings (lower values)
        for (int i = 0; i < 10; i++)
        {
            var reading = new SensorReading(
                fieldId,
                "Temperature",
                20m + i,
                "Celsius",
                baseTime.AddMinutes(i * 5)
            );
            _service.AddReading(reading);
        }

        // Add recent readings (higher values)
        for (int i = 0; i < 10; i++)
        {
            var reading = new SensorReading(
                fieldId,
                "Temperature",
                30m + i,
                "Celsius",
                baseTime.AddHours(1).AddMinutes(i * 5)
            );
            _service.AddReading(reading);
        }

        // Act
        var result = await _service.GetTrendAsync(fieldId, "Temperature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Increasing", result.Trend);
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Calculate_Correct_Stats()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var values = new[] { 20m, 25m, 30m, 35m, 40m };

        foreach (var value in values)
        {
            var reading = new SensorReading(
                fieldId,
                "Temperature",
                value,
                "Celsius",
                DateTime.UtcNow
            );
            _service.AddReading(reading);
        }

        // Act
        var result = await _service.GetStatisticsAsync(fieldId, "Temperature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.ReadingCount);
        Assert.Equal(20m, result.Min);
        Assert.Equal(40m, result.Max);
        Assert.InRange(result.Average!.Value, 29m, 31m); // Average should be around 30
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Return_Null_For_No_Readings()
    {
        // Arrange
        var fieldId = Guid.NewGuid();

        // Act
        var result = await _service.GetStatisticsAsync(fieldId, "Temperature");

        // Assert
        Assert.Null(result);
    }
}

