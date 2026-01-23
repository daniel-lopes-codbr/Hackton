using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Fields;

/// <summary>
/// Query to get fields by farm ID
/// </summary>
public class GetFieldsByFarmIdQuery : IRequest<IEnumerable<FieldDto>>
{
    public Guid FarmId { get; set; }
}
