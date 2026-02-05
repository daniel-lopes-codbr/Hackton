using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Farms;

/// <summary>
/// Query to get all farms for a specific user
/// </summary>
public class GetFarmsByUserIdQuery : IRequest<IEnumerable<FarmDto>>
{
    public Guid UserId { get; set; }
}
