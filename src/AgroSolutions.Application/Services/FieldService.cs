using AgroSolutions.Application.Commands.Fields;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Queries.Fields;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service implementation for Field management (uses MediatR internally)
/// </summary>
public class FieldService : IFieldService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IFieldRepository _repository; // Keep for simple queries
    private readonly ILogger<FieldService> _logger;

    public FieldService(
        IMediator mediator,
        IMapper mapper,
        IFieldRepository repository,
        ILogger<FieldService> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<FieldDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Use Query via MediatR
        var query = new GetAllFieldsQuery();
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<FieldDto>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        // Use Query via MediatR
        var query = new GetFieldsByFarmIdQuery { FarmId = farmId };
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<FieldDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Use Query via MediatR
        var query = new GetFieldByIdQuery { Id = id };
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<Result<FieldDto>> CreateFieldAsync(Guid farmId, CreateFieldDto dto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<CreateFieldCommand>(dto);
        command.FarmId = farmId; // Set FarmId from route parameter
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result<FieldDto>> UpdateFieldAsync(Guid id, UpdateFieldDto dto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<UpdateFieldCommand>(dto);
        command.Id = id; // Set ID from route parameter
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result> DeleteFieldAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Create Command
        var command = new DeleteFieldCommand { Id = id };
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }
}
