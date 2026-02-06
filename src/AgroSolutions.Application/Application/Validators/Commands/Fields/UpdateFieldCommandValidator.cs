using AgroSolutions.Application.Commands.Fields;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Fields;

/// <summary>
/// Validator for UpdateFieldCommand
/// </summary>
public class UpdateFieldCommandValidator : AbstractValidator<UpdateFieldCommand>
{
    public UpdateFieldCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Field ID is required");
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Field name must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.AreaSquareMeters)
            .GreaterThan(0).WithMessage("AreaSquareMeters must be greater than 0")
            .When(x => x.AreaSquareMeters.HasValue);

        RuleFor(x => x.CropType)
            .MaximumLength(100).WithMessage("Crop type must not exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CropType));
    }
}
