using AgroSolutions.Application.Commands.Alerts;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using AgroSolutions.Application.Queries.Alerts;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service implementation for Alert management (uses MediatR internally)
/// </summary>
public class AlertService : IAlertService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AlertService(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<Result<AlertCreationResponseDto>> CreateAlertsAsync(CancellationToken cancellationToken = default)
    {
        var command = new CreateAlertsCommand();
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result<int>> UpdateAlertsAsync(CancellationToken cancellationToken = default)
    {
        var command = new UpdateAlertsCommand();
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<IEnumerable<AlertDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetAllAlertsQuery();
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<AlertDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetAlertByIdQuery { Id = id };
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<AlertDto>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        var query = new GetAlertsByFieldIdQuery { FieldId = fieldId };
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<AlertDto>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        var query = new GetAlertsByFarmIdQuery { FarmId = farmId };
        return await _mediator.Send(query, cancellationToken);
    }
}
