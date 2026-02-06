using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Commands.Farms;

/// <summary>
/// Command to create a new farm
/// </summary>
public class CreateFarmCommand : IRequest<Result<FarmDto>>
{
    public string Name { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public decimal WidthMeters { get; set; }
    public decimal LengthMeters { get; set; }
    public decimal? TotalAreaSquareMeters { get; set; }
    public decimal? Precipitation { get; set; }
}
