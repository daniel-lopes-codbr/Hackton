using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Commands.Fields;

/// <summary>
/// Command to update an existing field
/// </summary>
public class UpdateFieldCommand : IRequest<Result<FieldDto>>
{
    public Guid Id { get; set; }
    public PropertyDto? Property { get; set; }
    public string? CropType { get; set; }
    public DateTime? PlantingDate { get; set; }
    public DateTime? HarvestDate { get; set; }
}
