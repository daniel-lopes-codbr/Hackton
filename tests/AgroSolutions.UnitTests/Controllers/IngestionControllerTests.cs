using AgroSolutions.Api.Controllers;
using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using AgroSolutions.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Api.Tests.Controllers;

public class IngestionControllerTests
{
    private readonly Mock<IIngestionService> _mockService;
    private readonly Mock<ILogger<IngestionController>> _mockLogger;
    private readonly IngestionController _controller;

    public IngestionControllerTests()
    {
        _mockService = new Mock<IIngestionService>();
        _mockLogger = new Mock<ILogger<IngestionController>>();
        _controller = new IngestionController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task IngestSingle_Should_Return_Created_Result()
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

        var resultDto = new SensorReadingDto
        {
            FieldId = dto.FieldId,
            SensorType = dto.SensorType,
            Value = dto.Value,
            Unit = dto.Unit,
            ReadingTimestamp = dto.ReadingTimestamp
        };

        var result = Result<SensorReadingDto>.Success(resultDto);

        _mockService.Setup(s => s.IngestSingleAsync(It.IsAny<SensorReadingDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.IngestSingle(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.NotNull(createdResult.Value);
        _mockService.Verify(s => s.IngestSingleAsync(It.IsAny<SensorReadingDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IngestBatch_Should_Return_Ok_Result()
    {
        // Arrange
        var batchDto = new BatchSensorReadingDto
        {
            Readings = new List<SensorReadingDto>
            {
                new() { FieldId = Guid.NewGuid(), SensorType = "Temperature", Value = 25.5m, Unit = "Celsius", ReadingTimestamp = DateTime.UtcNow }
            }
        };

        var response = new IngestionResponseDto
        {
            Success = true,
            ProcessedCount = 1,
            FailedCount = 0,
            ProcessingTime = TimeSpan.FromMilliseconds(10)
        };

        var result = Result<IngestionResponseDto>.Success(response);

        _mockService.Setup(s => s.IngestBatchAsync(It.IsAny<BatchSensorReadingDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.IngestBatch(batchDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var responseDto = Assert.IsType<IngestionResponseDto>(okResult.Value);
        Assert.True(responseDto.Success);
        Assert.Equal(1, responseDto.ProcessedCount);
    }

    [Fact]
    public async Task IngestBatchParallel_Should_Return_Ok_Result()
    {
        // Arrange
        var batchDto = new BatchSensorReadingDto
        {
            Readings = new List<SensorReadingDto>
            {
                new() { FieldId = Guid.NewGuid(), SensorType = "Temperature", Value = 25.5m, Unit = "Celsius", ReadingTimestamp = DateTime.UtcNow }
            }
        };

        var response = new IngestionResponseDto
        {
            Success = true,
            ProcessedCount = 1,
            FailedCount = 0,
            ProcessingTime = TimeSpan.FromMilliseconds(5)
        };

        var result = Result<IngestionResponseDto>.Success(response);

        _mockService.Setup(s => s.IngestBatchParallelAsync(It.IsAny<BatchSensorReadingDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.IngestBatchParallel(batchDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var responseDto = Assert.IsType<IngestionResponseDto>(okResult.Value);
        Assert.True(responseDto.Success);
    }

    [Fact]
    public void Health_Should_Return_Ok_Result()
    {
        // Act
        var result = _controller.Health();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}

