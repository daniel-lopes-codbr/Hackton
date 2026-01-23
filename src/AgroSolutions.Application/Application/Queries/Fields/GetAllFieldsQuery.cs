using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Fields;

/// <summary>
/// Query to get all fields
/// </summary>
public class GetAllFieldsQuery : IRequest<IEnumerable<FieldDto>>
{
}
