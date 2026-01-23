using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Farms;

/// <summary>
/// Query to get a farm by ID
/// </summary>
public class GetFarmByIdQuery : IRequest<FarmDto?>
{
    public Guid Id { get; set; }
}
