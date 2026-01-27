using AgroSolutions.Application.Models;
using AgroSolutions.Application.Queries.Alerts;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Alerts;

/// <summary>
/// Handler for GetAlertsByFieldIdQuery
/// </summary>
public class GetAlertsByFieldIdQueryHandler : IRequestHandler<GetAlertsByFieldIdQuery, IEnumerable<AlertDto>>
{
    private readonly IAlertRepository _repository;
    private readonly IMapper _mapper;

    public GetAlertsByFieldIdQueryHandler(IAlertRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlertDto>> Handle(GetAlertsByFieldIdQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _repository.GetByFieldIdAsync(request.FieldId, cancellationToken);
        return _mapper.Map<IEnumerable<AlertDto>>(alerts);
    }
}
