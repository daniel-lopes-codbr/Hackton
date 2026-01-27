using AgroSolutions.Application.Commands.Alerts;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Alerts;

/// <summary>
/// Validator for UpdateAlertsCommand
/// </summary>
public class UpdateAlertsCommandValidator : AbstractValidator<UpdateAlertsCommand>
{
    public UpdateAlertsCommandValidator()
    {
        // Command is empty, no validation needed
        // Validation happens in the handler
    }
}
