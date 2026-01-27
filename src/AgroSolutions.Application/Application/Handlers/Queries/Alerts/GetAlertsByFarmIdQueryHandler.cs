using AgroSolutions.Application.Models;
using AgroSolutions.Application.Queries.Alerts;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Alerts;

/// <summary>
/// Handler for GetAlertsByFarmIdQuery
/// </summary>
public class GetAlertsByFarmIdQueryHandler : IRequestHandler<GetAlertsByFarmIdQuery, IEnumerable<AlertDto>>
{
    private readonly IAlertRepository _repository;
    private readonly IMapper _mapper;

    public GetAlertsByFarmIdQueryHandler(IAlertRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlertDto>> Handle(GetAlertsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _repository.GetByFarmIdAsync(request.FarmId, cancellationToken);
        return _mapper.Map<IEnumerable<AlertDto>>(alerts);
    }
}
