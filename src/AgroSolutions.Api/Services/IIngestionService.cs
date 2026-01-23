using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Api.Models;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service interface for high-performance data ingestion
/// </summary>
public interface IIngestionService
{
    /// <summary>
    /// Ingests a single sensor reading
    /// </summary>
    Task<Result<SensorReadingDto>> IngestSingleAsync(SensorReadingDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple sensor readings in batch with high performance
    /// </summary>
    Task<Result<IngestionResponseDto>> IngestBatchAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple sensor readings in parallel for maximum performance
    /// </summary>
    Task<Result<IngestionResponseDto>> IngestBatchParallelAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default);
}
