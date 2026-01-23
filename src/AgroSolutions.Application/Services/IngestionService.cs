using AgroSolutions.Application.Commands.Ingestion;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Services;

/// <summary>
/// High-performance ingestion service for sensor data (uses MediatR internally)
/// </summary>
public class IngestionService : IIngestionService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<IngestionService> _logger;

    public IngestionService(
        IMediator mediator,
        IMapper mapper,
        ILogger<IngestionService> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SensorReadingDto>> IngestSingleAsync(SensorReadingDto dto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<IngestSingleCommand>(dto);
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result<IngestionResponseDto>> IngestBatchAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<IngestBatchCommand>(batchDto);
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result<IngestionResponseDto>> IngestBatchParallelAsync(BatchSensorReadingDto batchDto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<IngestBatchParallelCommand>(batchDto);
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }
}
