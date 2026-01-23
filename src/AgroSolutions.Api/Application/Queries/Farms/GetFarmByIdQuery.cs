using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Queries.Farms;

/// <summary>
/// Query to get a farm by ID
/// </summary>
public class GetFarmByIdQuery : IRequest<FarmDto?>
{
    public Guid Id { get; set; }
}
