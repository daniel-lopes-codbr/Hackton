using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Commands.Fields;

/// <summary>
/// Command to create a new field
/// </summary>
public class CreateFieldCommand : IRequest<Result<FieldDto>>
{
    public Guid FarmId { get; set; }
    public PropertyDto Property { get; set; } = null!;
    public string CropType { get; set; } = string.Empty;
}
