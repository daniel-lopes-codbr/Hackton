using AgroSolutions.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Application.Commands.Alerts;

/// <summary>
/// Command to deactivate alerts created the previous day
/// </summary>
public class UpdateAlertsCommand : IRequest<Result<int>>
{
    // Empty - deactivates all alerts from previous day
}
