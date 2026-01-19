using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service interface for high-performance data ingestion
/// </summary>
public interface IIngestionService
{
    /// <summary>
    /// Ingests a single sensor reading
    /// </summary>
    Task<SensorReading> IngestSingleAsync(SensorReadingDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple sensor readings in batch with high performance
    /// </summary>
    Task<IngestionResponseDto> IngestBatchAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple sensor readings in parallel for maximum performance
    /// </summary>
    Task<IngestionResponseDto> IngestBatchParallelAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all ingested readings (for health checks and testing)
    /// </summary>
    IEnumerable<SensorReading> GetAllReadings();
}
