using AgroSolutions.Application.Commands.Ingestion;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Repositories;
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
    private readonly ISensorReadingRepository _repository;

    public IngestionService(
        IMediator mediator,
        IMapper mapper,
        ILogger<IngestionService> logger,
        ISensorReadingRepository repository)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
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

    public async Task<SensorReadingDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reading = await _repository.GetByIdAsync(id, cancellationToken);
        return reading == null ? null : _mapper.Map<SensorReadingDto>(reading);
    }

    public async Task<IEnumerable<SensorReadingDto>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        var readings = await _repository.GetByFieldIdAsync(fieldId, cancellationToken);
        return _mapper.Map<IEnumerable<SensorReadingDto>>(readings);
    }
}
