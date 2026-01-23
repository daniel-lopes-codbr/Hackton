using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Queries.Fields;

/// <summary>
/// Query to get all fields
/// </summary>
public class GetAllFieldsQuery : IRequest<IEnumerable<FieldDto>>
{
}
