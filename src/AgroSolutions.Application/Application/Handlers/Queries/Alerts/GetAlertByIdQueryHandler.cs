using AgroSolutions.Application.Models;
using AgroSolutions.Application.Queries.Alerts;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Alerts;

/// <summary>
/// Handler for GetAlertByIdQuery
/// </summary>
public class GetAlertByIdQueryHandler : IRequestHandler<GetAlertByIdQuery, AlertDto?>
{
    private readonly IAlertRepository _repository;
    private readonly IMapper _mapper;

    public GetAlertByIdQueryHandler(IAlertRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AlertDto?> Handle(GetAlertByIdQuery request, CancellationToken cancellationToken)
    {
        var alert = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return alert == null ? null : _mapper.Map<AlertDto>(alert);
    }
}
