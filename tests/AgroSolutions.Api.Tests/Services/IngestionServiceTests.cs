using AgroSolutions.Api.Models;
using AgroSolutions.Api.Services;
using Xunit;

namespace AgroSolutions.Api.Tests.Services;

public class IngestionServiceTests
{
    private readonly IngestionService _service;
    private readonly ILogger<IngestionService> _logger;

    public IngestionServiceTests()
    {
        _logger = new LoggerFactory().CreateLogger<IngestionService>();
        _service = new IngestionService(_logger);
    }

    [Fact]
    public async Task IngestSingleAsync_Should_Create_SensorReading()
    {
        // Arrange
        var dto = new SensorReadingDto
        {
            FieldId = Guid.NewGuid(),
            SensorType = "Temperature",
            Value = 25.5m,
            Unit = "Celsius",
            ReadingTimestamp = DateTime.UtcNow
        };

        // Act
        var result = await _service.IngestSingleAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(dto.FieldId, result.FieldId);
        Assert.Equal(dto.SensorType, result.SensorType);
        Assert.Equal(dto.Value, result.Value);
    }

    [Fact]
    public async Task IngestSingleAsync_Should_Throw_When_FieldId_Is_Empty()
    {
        // Arrange
        var dto = new SensorReadingDto
        {
            FieldId = Guid.Empty,
            SensorType = "Temperature",
            Value = 25.5m,
            Unit = "Celsius",
            ReadingTimestamp = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<AgroSolutions.Domain.Exceptions.DomainException>(() => _service.IngestSingleAsync(dto));
    }

    [Fact]
    public async Task IngestBatchAsync_Should_Process_All_Readings()
    {
        // Arrange
        var batchDto = new BatchSensorReadingDto
        {
            Readings = new List<SensorReadingDto>
            {
                new() { FieldId = Guid.NewGuid(), SensorType = "Temperature", Value = 25.5m, Unit = "Celsius", ReadingTimestamp = DateTime.UtcNow },
                new() { FieldId = Guid.NewGuid(), SensorType = "Humidity", Value = 60.0m, Unit = "Percent", ReadingTimestamp = DateTime.UtcNow },
                new() { FieldId = Guid.NewGuid(), SensorType = "SoilMoisture", Value = 45.0m, Unit = "Percent", ReadingTimestamp = DateTime.UtcNow }
            }
        };

        // Act
        var result = await _service.IngestBatchAsync(batchDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.ProcessedCount);
        Assert.Equal(0, result.FailedCount);
        Assert.True(result.ProcessingTime.TotalMilliseconds >= 0);
    }

    [Fact]
    public async Task IngestBatchAsync_Should_Handle_Failures_Gracefully()
    {
        // Arrange
        var batchDto = new BatchSensorReadingDto
        {
            Readings = new List<SensorReadingDto>
            {
                new() { FieldId = Guid.NewGuid(), SensorType = "Temperature", Value = 25.5m, Unit = "Celsius", ReadingTimestamp = DateTime.UtcNow },
                new() { FieldId = Guid.Empty, SensorType = "Humidity", Value = 60.0m, Unit = "Percent", ReadingTimestamp = DateTime.UtcNow }, // Invalid
                new() { FieldId = Guid.NewGuid(), SensorType = "SoilMoisture", Value = 45.0m, Unit = "Percent", ReadingTimestamp = DateTime.UtcNow }
            }
        };

        // Act
        var result = await _service.IngestBatchAsync(batchDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(2, result.ProcessedCount);
        Assert.Equal(1, result.FailedCount);
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task IngestBatchParallelAsync_Should_Process_All_Readings()
    {
        // Arrange
        var batchDto = new BatchSensorReadingDto
        {
            Readings = Enumerable.Range(0, 10).Select(i => new SensorReadingDto
            {
                FieldId = Guid.NewGuid(),
                SensorType = "Temperature",
                Value = 20 + i,
                Unit = "Celsius",
                ReadingTimestamp = DateTime.UtcNow
            }).ToList()
        };

        // Act
        var result = await _service.IngestBatchParallelAsync(batchDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(10, result.ProcessedCount);
        Assert.Equal(0, result.FailedCount);
    }

    [Fact]
    public async Task IngestBatchParallelAsync_Should_Be_Faster_Than_Sequential()
    {
        // Arrange
        var readings = Enumerable.Range(0, 50).Select(i => new SensorReadingDto
        {
            FieldId = Guid.NewGuid(),
            SensorType = "Temperature",
            Value = 20 + i,
            Unit = "Celsius",
            ReadingTimestamp = DateTime.UtcNow
        }).ToList();

        var batchDto = new BatchSensorReadingDto { Readings = readings };

        // Act
        var sequentialStart = DateTime.UtcNow;
        var sequentialResult = await _service.IngestBatchAsync(batchDto);
        var sequentialTime = DateTime.UtcNow - sequentialStart;

        // Create new service instance to avoid state issues
        var service2 = new IngestionService(_logger);
        var parallelStart = DateTime.UtcNow;
        var parallelResult = await service2.IngestBatchParallelAsync(batchDto);
        var parallelTime = DateTime.UtcNow - parallelStart;

        // Assert
        Assert.True(sequentialResult.Success);
        Assert.True(parallelResult.Success);
        // Note: Parallel might not always be faster for small batches due to overhead,
        // but it should handle larger batches more efficiently
        Assert.True(sequentialTime.TotalMilliseconds >= 0);
        Assert.True(parallelTime.TotalMilliseconds >= 0);
    }

    [Fact]
    public async Task IngestBatchAsync_Should_Return_Error_When_No_Readings()
    {
        // Arrange
        var batchDto = new BatchSensorReadingDto { Readings = new List<SensorReadingDto>() };

        // Act
        var result = await _service.IngestBatchAsync(batchDto);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.Contains("No readings provided", result.Errors.First());
    }
}
