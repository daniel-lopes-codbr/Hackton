using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Commands.Ingestion;

/// <summary>
/// Command to ingest multiple sensor readings in batch
/// </summary>
public class IngestBatchCommand : IRequest<Result<IngestionResponseDto>>
{
    public BatchSensorReadingDto Batch { get; set; } = null!;
}
