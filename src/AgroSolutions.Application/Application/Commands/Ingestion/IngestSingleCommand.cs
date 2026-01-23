using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Commands.Ingestion;

/// <summary>
/// Command to ingest a single sensor reading
/// </summary>
public class IngestSingleCommand : IRequest<Result<SensorReadingDto>>
{
    public SensorReadingDto Reading { get; set; } = null!;
}
