using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Alerts;

/// <summary>
/// Query to get alerts by FieldId
/// </summary>
public class GetAlertsByFieldIdQuery : IRequest<IEnumerable<AlertDto>>
{
    public Guid FieldId { get; set; }
}
