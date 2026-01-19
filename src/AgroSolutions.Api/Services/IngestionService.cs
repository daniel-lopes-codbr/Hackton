using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Exceptions;

namespace AgroSolutions.Api.Services;

/// <summary>
/// High-performance ingestion service for sensor data
/// </summary>
public class IngestionService : IIngestionService
{
    private readonly ILogger<IngestionService> _logger;
    private readonly List<SensorReading> _inMemoryStore; // In-memory store for FASE 2 (will be replaced with database in later phases)

    public IngestionService(ILogger<IngestionService> logger)
    {
        _logger = logger;
        _inMemoryStore = new List<SensorReading>();
    }

    public Task<SensorReading> IngestSingleAsync(SensorReadingDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var reading = new SensorReading(
                dto.FieldId,
                dto.SensorType,
                dto.Value,
                dto.Unit,
                dto.ReadingTimestamp,
                dto.Location,
                dto.Metadata
            );

            _inMemoryStore.Add(reading);
            _logger.LogInformation("Ingested single reading: {SensorType} for Field {FieldId}", dto.SensorType, dto.FieldId);

            return Task.FromResult(reading);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting single reading for Field {FieldId}", dto.FieldId);
            throw new DomainException($"Failed to ingest sensor reading: {ex.Message}", ex);
        }
    }

    public async Task<IngestionResponseDto> IngestBatchAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var response = new IngestionResponseDto
        {
            Success = true,
            Errors = new List<string>()
        };

        if (batchDto.Readings == null || batchDto.Readings.Count == 0)
        {
            response.Success = false;
            response.Errors!.Add("No readings provided in batch");
            return response;
        }

        foreach (var dto in batchDto.Readings)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await IngestSingleAsync(dto, cancellationToken);
                response.ProcessedCount++;
            }
            catch (Exception ex)
            {
                response.FailedCount++;
                response.Errors!.Add($"Field {dto.FieldId}: {ex.Message}");
                _logger.LogWarning(ex, "Failed to ingest reading for Field {FieldId}", dto.FieldId);
            }
        }

        response.ProcessingTime = DateTime.UtcNow - startTime;
        response.Success = response.FailedCount == 0;

        _logger.LogInformation(
            "Batch ingestion completed: {ProcessedCount} processed, {FailedCount} failed in {ProcessingTime}ms",
            response.ProcessedCount,
            response.FailedCount,
            response.ProcessingTime.TotalMilliseconds);

        return response;
    }

    public async Task<IngestionResponseDto> IngestBatchParallelAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var processedCount = 0;
        var failedCount = 0;
        var errors = new List<string>();

        if (batchDto.Readings == null || batchDto.Readings.Count == 0)
        {
            return new IngestionResponseDto
            {
                Success = false,
                Errors = new List<string> { "No readings provided in batch" },
                ProcessingTime = DateTime.UtcNow - startTime
            };
        }

        // Use Parallel.ForEach for high-performance parallel processing
        var semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);
        var tasks = new List<Task>();

        foreach (var dto in batchDto.Readings)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var task = Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await IngestSingleAsync(dto, cancellationToken);
                    Interlocked.Increment(ref processedCount);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref failedCount);
                    lock (errors)
                    {
                        errors.Add($"Field {dto.FieldId}: {ex.Message}");
                    }
                    _logger.LogWarning(ex, "Failed to ingest reading for Field {FieldId}", dto.FieldId);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        
        var response = new IngestionResponseDto
        {
            Success = failedCount == 0,
            ProcessedCount = processedCount,
            FailedCount = failedCount,
            Errors = errors,
            ProcessingTime = DateTime.UtcNow - startTime
        };

        _logger.LogInformation(
            "Parallel batch ingestion completed: {ProcessedCount} processed, {FailedCount} failed in {ProcessingTime}ms",
            response.ProcessedCount,
            response.FailedCount,
            response.ProcessingTime.TotalMilliseconds);

        return response;
    }

    // Helper method to get all readings (for testing/validation)
    public IEnumerable<SensorReading> GetAllReadings()
    {
        return _inMemoryStore.AsEnumerable();
    }
}
