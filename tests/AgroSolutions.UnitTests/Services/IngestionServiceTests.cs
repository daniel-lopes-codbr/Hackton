using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using AgroSolutions.Application.Commands.Ingestion;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using MediatR;
using Xunit;

namespace AgroSolutions.Api.Tests.Services;

public class IngestionServiceTests
{
    private readonly IngestionService _service;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ISensorReadingRepository> _mockRepository;
    private readonly ILogger<IngestionService> _logger;

    public IngestionServiceTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockMapper = new Mock<IMapper>();
        _mockRepository = new Mock<ISensorReadingRepository>();
        _logger = new LoggerFactory().CreateLogger<IngestionService>();
        _service = new IngestionService(_mockMediator.Object, _mockMapper.Object, _logger, _mockRepository.Object);
    }

    [Fact]
    public async Task IngestSingleAsync_Should_Return_Success_Result()
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

        var expectedDto = new SensorReadingDto
        {
            FieldId = dto.FieldId,
            SensorType = dto.SensorType,
            Value = dto.Value,
            Unit = dto.Unit,
            ReadingTimestamp = dto.ReadingTimestamp
        };

        var command = new IngestSingleCommand { Reading = dto };
        var expectedResult = Result<SensorReadingDto>.Success(expectedDto);

        _mockMapper.Setup(m => m.Map<IngestSingleCommand>(dto)).Returns(command);
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.IngestSingleAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(dto.FieldId, result.Value.FieldId);
        Assert.Equal(dto.SensorType, result.Value.SensorType);
        Assert.Equal(dto.Value, result.Value.Value);
    }

    [Fact]
    public async Task IngestSingleAsync_Should_Return_Failure_When_Validation_Fails()
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

        var command = new IngestSingleCommand { Reading = dto };
        var expectedResult = Result<SensorReadingDto>.Failure("FieldId", "Field ID is required");

        _mockMapper.Setup(m => m.Map<IngestSingleCommand>(dto)).Returns(command);
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.IngestSingleAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
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

        var command = new IngestBatchCommand { Batch = batchDto };
        var responseDto = new IngestionResponseDto
        {
            Success = true,
            ProcessedCount = 3,
            FailedCount = 0,
            ProcessingTime = TimeSpan.FromMilliseconds(100)
        };
        var expectedResult = Result<IngestionResponseDto>.Success(responseDto);

        _mockMapper.Setup(m => m.Map<IngestBatchCommand>(batchDto)).Returns(command);
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.IngestBatchAsync(batchDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Success);
        Assert.Equal(3, result.Value.ProcessedCount);
        Assert.Equal(0, result.Value.FailedCount);
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

        var command = new IngestBatchCommand { Batch = batchDto };
        var expectedResult = Result<IngestionResponseDto>.Failure("Validation", "Some readings failed validation");

        _mockMapper.Setup(m => m.Map<IngestBatchCommand>(batchDto)).Returns(command);
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.IngestBatchAsync(batchDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
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

        var command = new IngestBatchParallelCommand { Batch = batchDto };
        var responseDto = new IngestionResponseDto
        {
            Success = true,
            ProcessedCount = 10,
            FailedCount = 0,
            ProcessingTime = TimeSpan.FromMilliseconds(50)
        };
        var expectedResult = Result<IngestionResponseDto>.Success(responseDto);

        _mockMapper.Setup(m => m.Map<IngestBatchParallelCommand>(batchDto)).Returns(command);
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.IngestBatchParallelAsync(batchDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.ProcessedCount);
        Assert.Equal(0, result.Value.FailedCount);
    }

    [Fact]
    public async Task IngestBatchParallelAsync_Should_Call_MediatR_With_Parallel_Command()
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
        var command = new IngestBatchParallelCommand { Batch = batchDto };
        var responseDto = new IngestionResponseDto
        {
            Success = true,
            ProcessedCount = 50,
            FailedCount = 0,
            ProcessingTime = TimeSpan.FromMilliseconds(30)
        };
        var expectedResult = Result<IngestionResponseDto>.Success(responseDto);

        _mockMapper.Setup(m => m.Map<IngestBatchParallelCommand>(batchDto)).Returns(command);
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.IngestBatchParallelAsync(batchDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        _mockMediator.Verify(m => m.Send(It.Is<IngestBatchParallelCommand>(c => c.Batch == batchDto), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IngestBatchAsync_Should_Return_Error_When_No_Readings()
    {
        // Arrange
        var batchDto = new BatchSensorReadingDto { Readings = new List<SensorReadingDto>() };
        var command = new IngestBatchCommand { Batch = batchDto };
        var expectedResult = Result<IngestionResponseDto>.Failure("Batch", "No readings provided in batch");

        _mockMapper.Setup(m => m.Map<IngestBatchCommand>(batchDto)).Returns(command);
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.IngestBatchAsync(batchDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
    }
}

