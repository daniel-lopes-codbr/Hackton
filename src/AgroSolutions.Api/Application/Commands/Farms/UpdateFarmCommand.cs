using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Commands.Farms;

/// <summary>
/// Command to update an existing farm
/// </summary>
public class UpdateFarmCommand : IRequest<Result<FarmDto>>
{
    public Guid Id { get; set; }
    public PropertyDto? Property { get; set; }
    public string? OwnerName { get; set; }
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
}
