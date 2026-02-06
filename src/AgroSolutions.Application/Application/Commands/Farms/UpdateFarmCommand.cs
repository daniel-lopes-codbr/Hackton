using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Commands.Farms;

/// <summary>
/// Command to update an existing farm
/// </summary>
public class UpdateFarmCommand : IRequest<Result<FarmDto>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid? UserId { get; set; }
    public decimal? WidthMeters { get; set; }
    public decimal? LengthMeters { get; set; }
    public decimal? TotalAreaSquareMeters { get; set; }
    public decimal? Precipitation { get; set; }
}
