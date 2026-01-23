using AgroSolutions.Api.Application.Queries.Farms;
using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Api.Application.Handlers.Queries.Farms;

/// <summary>
/// Handler for GetFarmByIdQuery
/// </summary>
public class GetFarmByIdQueryHandler : IRequestHandler<GetFarmByIdQuery, FarmDto?>
{
    private readonly IFarmRepository _repository;
    private readonly IMapper _mapper;

    public GetFarmByIdQueryHandler(IFarmRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<FarmDto?> Handle(GetFarmByIdQuery request, CancellationToken cancellationToken)
    {
        var farm = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return farm == null ? null : _mapper.Map<FarmDto>(farm);
    }
}
