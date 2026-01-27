using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Alerts;

/// <summary>
/// Query to get alerts by FarmId
/// </summary>
public class GetAlertsByFarmIdQuery : IRequest<IEnumerable<AlertDto>>
{
    public Guid FarmId { get; set; }
}
