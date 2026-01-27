using AgroSolutions.Application.Commands.Alerts;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Alerts;

/// <summary>
/// Validator for CreateAlertsCommand
/// </summary>
public class CreateAlertsCommandValidator : AbstractValidator<CreateAlertsCommand>
{
    public CreateAlertsCommandValidator()
    {
        // Command is empty, no validation needed
        // Validation happens in the handler based on sensor data
    }
}
