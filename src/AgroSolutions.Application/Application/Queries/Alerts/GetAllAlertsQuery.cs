using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Alerts;

/// <summary>
/// Query to get all alerts
/// </summary>
public class GetAllAlertsQuery : IRequest<IEnumerable<AlertDto>>
{
}
