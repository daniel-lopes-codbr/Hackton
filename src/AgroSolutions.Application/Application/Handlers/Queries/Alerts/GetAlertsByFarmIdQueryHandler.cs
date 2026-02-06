using AgroSolutions.Application.Models;
using AgroSolutions.Application.Queries.Alerts;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Alerts;

/// <summary>
/// Handler for GetAlertsByFarmIdQuery
/// </summary>
public class GetAlertsByFarmIdQueryHandler : IRequestHandler<GetAlertsByFarmIdQuery, IEnumerable<AlertDto>>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public GetAlertsByFarmIdQueryHandler(IAlertRepository alertRepository, IFieldRepository fieldRepository, IMapper mapper)
    {
        _alertRepository = alertRepository;
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlertDto>> Handle(GetAlertsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        var fields = await _fieldRepository.GetByFarmIdAsync(request.FarmId, cancellationToken);
        var alerts = new List<Alert>();

        foreach (var f in fields)
        {
            var fieldAlerts = await _alertRepository.GetByFieldIdAsync(f.Id, cancellationToken);
            alerts.AddRange(fieldAlerts);
        }

        return _mapper.Map<IEnumerable<AlertDto>>(alerts);
    }
}
