using AgroSolutions.Api.Application.Commands.Fields;
using FluentValidation;

namespace AgroSolutions.Api.Application.Validators.Commands.Fields;

/// <summary>
/// Validator for CreateFieldCommand
/// </summary>
public class CreateFieldCommandValidator : AbstractValidator<CreateFieldCommand>
{
    public CreateFieldCommandValidator()
    {
        RuleFor(x => x.FarmId)
            .NotEmpty().WithMessage("Farm ID is required");

        RuleFor(x => x.Property)
            .NotNull().WithMessage("Property is required");

        RuleFor(x => x.Property.Name)
            .NotEmpty().WithMessage("Property name is required")
            .MaximumLength(200).WithMessage("Property name must not exceed 200 characters")
            .When(x => x.Property != null);

        RuleFor(x => x.Property.Location)
            .NotEmpty().WithMessage("Property location is required")
            .MaximumLength(500).WithMessage("Property location must not exceed 500 characters")
            .When(x => x.Property != null);

        RuleFor(x => x.Property.Area)
            .GreaterThan(0).WithMessage("Property area must be greater than 0")
            .When(x => x.Property != null);

        RuleFor(x => x.Property.Description)
            .MaximumLength(1000).WithMessage("Property description must not exceed 1000 characters")
            .When(x => x.Property != null && !string.IsNullOrWhiteSpace(x.Property.Description));

        RuleFor(x => x.CropType)
            .NotEmpty().WithMessage("Crop type is required")
            .MaximumLength(100).WithMessage("Crop type must not exceed 100 characters");
    }
}
