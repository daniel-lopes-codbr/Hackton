using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Commands.Ingestion;

/// <summary>
/// Command to ingest a single sensor reading
/// </summary>
public class IngestSingleCommand : IRequest<Result<SensorReadingDto>>
{
    public SensorReadingDto Reading { get; set; } = null!;
}
