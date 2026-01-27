using AgroSolutions.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Application.Commands.Alerts;

/// <summary>
/// Command to create alerts based on sensor readings from the last hour
/// </summary>
public class CreateAlertsCommand : IRequest<Result<Models.AlertCreationResponseDto>>
{
    // Empty - triggers automatic alert generation
}
