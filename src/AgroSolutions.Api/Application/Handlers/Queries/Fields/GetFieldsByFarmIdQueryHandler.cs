using AgroSolutions.Api.Application.Queries.Fields;
using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Api.Application.Handlers.Queries.Fields;

/// <summary>
/// Handler for GetFieldsByFarmIdQuery
/// </summary>
public class GetFieldsByFarmIdQueryHandler : IRequestHandler<GetFieldsByFarmIdQuery, IEnumerable<FieldDto>>
{
    private readonly IFieldRepository _repository;
    private readonly IMapper _mapper;

    public GetFieldsByFarmIdQueryHandler(IFieldRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FieldDto>> Handle(GetFieldsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        var fields = await _repository.GetByFarmIdAsync(request.FarmId, cancellationToken);
        return fields.Select(f => _mapper.Map<FieldDto>(f));
    }
}
