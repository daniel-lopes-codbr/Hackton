using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Commands.Ingestion;

/// <summary>
/// Command to ingest multiple sensor readings in parallel
/// </summary>
public class IngestBatchParallelCommand : IRequest<Result<IngestionResponseDto>>
{
    public BatchSensorReadingDto Batch { get; set; } = null!;
}
