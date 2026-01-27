using AgroSolutions.Application.Models;
using AgroSolutions.Application.Queries.Alerts;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Alerts;

/// <summary>
/// Handler for GetAllAlertsQuery
/// </summary>
public class GetAllAlertsQueryHandler : IRequestHandler<GetAllAlertsQuery, IEnumerable<AlertDto>>
{
    private readonly IAlertRepository _repository;
    private readonly IMapper _mapper;

    public GetAllAlertsQueryHandler(IAlertRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlertDto>> Handle(GetAllAlertsQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<AlertDto>>(alerts);
    }
}
