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
    public string? Name { get; set; }
    public decimal? AreaSquareMeters { get; set; }
    public string? CropType { get; set; }
}
