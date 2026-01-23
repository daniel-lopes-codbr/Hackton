using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Farms;

/// <summary>
/// Query to get all farms
/// </summary>
public class GetAllFarmsQuery : IRequest<IEnumerable<FarmDto>>
{
}
