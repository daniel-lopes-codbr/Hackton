using AgroSolutions.Application.Queries.Fields;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Fields;

/// <summary>
/// Handler for GetAllFieldsQuery
/// </summary>
public class GetAllFieldsQueryHandler : IRequestHandler<GetAllFieldsQuery, IEnumerable<FieldDto>>
{
    private readonly IFieldRepository _repository;
    private readonly IMapper _mapper;

    public GetAllFieldsQueryHandler(IFieldRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FieldDto>> Handle(GetAllFieldsQuery request, CancellationToken cancellationToken)
    {
        var fields = await _repository.GetAllAsync(cancellationToken);
        return fields.Select(f => _mapper.Map<FieldDto>(f));
    }
}
