using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Queries.Farms;

/// <summary>
/// Query to get all farms
/// </summary>
public class GetAllFarmsQuery : IRequest<IEnumerable<FarmDto>>
{
}
