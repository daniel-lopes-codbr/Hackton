using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Commands.Farms;

/// <summary>
/// Command to create a new farm
/// </summary>
public class CreateFarmCommand : IRequest<Result<FarmDto>>
{
    public PropertyDto Property { get; set; } = null!;
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
}
