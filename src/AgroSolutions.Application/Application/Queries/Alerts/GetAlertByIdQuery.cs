using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Alerts;

/// <summary>
/// Query to get an alert by ID
/// </summary>
public class GetAlertByIdQuery : IRequest<AlertDto?>
{
    public Guid Id { get; set; }
}
