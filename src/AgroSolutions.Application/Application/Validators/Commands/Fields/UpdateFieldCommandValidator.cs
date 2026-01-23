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

        When(x => x.Property != null, () =>
        {
            RuleFor(x => x.Property!.Name)
                .NotEmpty().WithMessage("Property name is required")
                .MaximumLength(200).WithMessage("Property name must not exceed 200 characters");

            RuleFor(x => x.Property!.Location)
                .NotEmpty().WithMessage("Property location is required")
                .MaximumLength(500).WithMessage("Property location must not exceed 500 characters");

            RuleFor(x => x.Property!.Area)
                .GreaterThan(0).WithMessage("Property area must be greater than 0");

            RuleFor(x => x.Property!.Description)
                .MaximumLength(1000).WithMessage("Property description must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Property!.Description));
        });

        RuleFor(x => x.CropType)
            .MaximumLength(100).WithMessage("Crop type must not exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CropType));
    }
}
