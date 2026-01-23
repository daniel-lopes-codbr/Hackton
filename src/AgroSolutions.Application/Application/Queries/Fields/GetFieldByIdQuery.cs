using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Fields;

/// <summary>
/// Query to get a field by ID
/// </summary>
public class GetFieldByIdQuery : IRequest<FieldDto?>
{
    public Guid Id { get; set; }
}
