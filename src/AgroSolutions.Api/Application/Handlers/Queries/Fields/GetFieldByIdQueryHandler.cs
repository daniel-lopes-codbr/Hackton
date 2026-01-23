using AgroSolutions.Api.Application.Queries.Fields;
using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Api.Application.Handlers.Queries.Fields;

/// <summary>
/// Handler for GetFieldByIdQuery
/// </summary>
public class GetFieldByIdQueryHandler : IRequestHandler<GetFieldByIdQuery, FieldDto?>
{
    private readonly IFieldRepository _repository;
    private readonly IMapper _mapper;

    public GetFieldByIdQueryHandler(IFieldRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<FieldDto?> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        var field = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return field == null ? null : _mapper.Map<FieldDto>(field);
    }
}
