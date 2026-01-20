using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Exceptions;
using AgroSolutions.Domain.Repositories;

namespace AgroSolutions.Api.Services;

/// <summary>
/// High-performance ingestion service for sensor data
/// </summary>
public class IngestionService : IIngestionService
{
    private readonly ILogger<IngestionService> _logger;
    private readonly ISensorReadingRepository _repository;

    public IngestionService(ILogger<IngestionService> logger, ISensorReadingRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<SensorReading> IngestSingleAsync(SensorReadingDto dto, CancellationToken cancellationToken = default)
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

            await _repository.AddAsync(reading, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Ingested single reading: {SensorType} for Field {FieldId}", dto.SensorType, dto.FieldId);

            return reading;
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

        var readingsToAdd = new List<SensorReading>();

        foreach (var dto in batchDto.Readings)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

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
                readingsToAdd.Add(reading);
                response.ProcessedCount++;
            }
            catch (Exception ex)
            {
                response.FailedCount++;
                response.Errors!.Add($"Field {dto.FieldId}: {ex.Message}");
                _logger.LogWarning(ex, "Failed to create reading for Field {FieldId}", dto.FieldId);
            }
        }

        // Add all readings in batch
        if (readingsToAdd.Any())
        {
            try
            {
                await _repository.AddRangeAsync(readingsToAdd, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving batch to database");
                response.Success = false;
                response.Errors!.Add($"Database error: {ex.Message}");
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

        // Create readings in parallel (validation phase)
        var readingsToAdd = new List<SensorReading>();
        var semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);
        var tasks = new List<Task>();

        foreach (var dto in batchDto.Readings)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var task = Task.Run(() =>
            {
                semaphore.Wait(cancellationToken);
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
                    lock (readingsToAdd)
                    {
                        readingsToAdd.Add(reading);
                    }
                    Interlocked.Increment(ref processedCount);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref failedCount);
                    lock (errors)
                    {
                        errors.Add($"Field {dto.FieldId}: {ex.Message}");
                    }
                    _logger.LogWarning(ex, "Failed to create reading for Field {FieldId}", dto.FieldId);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Save all readings in batch to database
        if (readingsToAdd.Any())
        {
            try
            {
                await _repository.AddRangeAsync(readingsToAdd, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving parallel batch to database");
                failedCount += readingsToAdd.Count;
                lock (errors)
                {
                    errors.Add($"Database error: {ex.Message}");
                }
            }
        }
        
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
    public async Task<IEnumerable<SensorReading>> GetAllReadingsAsync(CancellationToken cancellationToken = default)
    {
        // This is a simple implementation - in production, you might want pagination
        // For now, we'll get readings from a sample field or return empty
        return Enumerable.Empty<SensorReading>();
    }

    // For backward compatibility with health checks
    public IEnumerable<SensorReading> GetAllReadings()
    {
        // Note: This is synchronous and may not work well with async repository
        // Consider updating health checks to use async methods
        // For now, return empty to avoid blocking
        return Enumerable.Empty<SensorReading>();
    }
}
