using AgroSolutions.Application.Commands.Fields;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Fields;

/// <summary>
/// Validator for CreateFieldCommand
/// </summary>
public class CreateFieldCommandValidator : AbstractValidator<CreateFieldCommand>
{
    public CreateFieldCommandValidator()
    {
        RuleFor(x => x.FarmId)
            .NotEmpty().WithMessage("Farm ID is required");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Field name is required")
            .MaximumLength(200).WithMessage("Field name must not exceed 200 characters");

        RuleFor(x => x.AreaSquareMeters)
            .GreaterThan(0).WithMessage("AreaSquareMeters must be greater than 0");

        RuleFor(x => x.CropType)
            .NotEmpty().WithMessage("Crop type is required")
            .MaximumLength(100).WithMessage("Crop type must not exceed 100 characters");
    }
}
