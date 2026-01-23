using AgroSolutions.Api.Application.Commands.Farms;
using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Api.Application.Queries.Farms;
using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service implementation for Farm management (uses MediatR internally)
/// </summary>
public class FarmService : IFarmService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IFarmRepository _repository; // Keep for simple queries
    private readonly ILogger<FarmService> _logger;

    public FarmService(
        IMediator mediator,
        IMapper mapper,
        IFarmRepository repository,
        ILogger<FarmService> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<FarmDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Use Query via MediatR
        var query = new GetAllFarmsQuery();
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<FarmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Use Query via MediatR
        var query = new GetFarmByIdQuery { Id = id };
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<Result<FarmDto>> CreateFarmAsync(CreateFarmDto dto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<CreateFarmCommand>(dto);
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result<FarmDto>> UpdateFarmAsync(Guid id, UpdateFarmDto dto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<UpdateFarmCommand>(dto);
        command.Id = id; // Set ID from route parameter
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result> DeleteFarmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Create Command
        var command = new DeleteFarmCommand { Id = id };
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }
}
