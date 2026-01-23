using AgroSolutions.Application.Queries.Farms;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Farms;

/// <summary>
/// Handler for GetAllFarmsQuery
/// </summary>
public class GetAllFarmsQueryHandler : IRequestHandler<GetAllFarmsQuery, IEnumerable<FarmDto>>
{
    private readonly IFarmRepository _repository;
    private readonly IMapper _mapper;

    public GetAllFarmsQueryHandler(IFarmRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FarmDto>> Handle(GetAllFarmsQuery request, CancellationToken cancellationToken)
    {
        var farms = await _repository.GetAllAsync(cancellationToken);
        return farms.Select(f => _mapper.Map<FarmDto>(f));
    }
}
