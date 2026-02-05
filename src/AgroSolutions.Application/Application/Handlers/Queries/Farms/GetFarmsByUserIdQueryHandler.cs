using AgroSolutions.Application.Queries.Farms;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Farms;

/// <summary>
/// Handler for GetFarmsByUserIdQuery
/// </summary>
public class GetFarmsByUserIdQueryHandler : IRequestHandler<GetFarmsByUserIdQuery, IEnumerable<FarmDto>>
{
    private readonly IFarmRepository _repository;
    private readonly IMapper _mapper;

    public GetFarmsByUserIdQueryHandler(IFarmRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FarmDto>> Handle(GetFarmsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var farms = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        return farms.Select(f => _mapper.Map<FarmDto>(f));
    }
}
