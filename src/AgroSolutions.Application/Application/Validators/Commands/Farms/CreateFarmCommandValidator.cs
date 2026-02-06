using AgroSolutions.Application.Commands.Farms;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Farms;

/// <summary>
/// Validator for CreateFarmCommand
/// </summary>
public class CreateFarmCommandValidator : AbstractValidator<CreateFarmCommand>
{
    public CreateFarmCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Farm name is required")
            .MaximumLength(200).WithMessage("Farm name must not exceed 200 characters");

        RuleFor(x => x.WidthMeters)
            .GreaterThan(0).WithMessage("WidthMeters must be greater than 0");

        RuleFor(x => x.LengthMeters)
            .GreaterThan(0).WithMessage("LengthMeters must be greater than 0");

        RuleFor(x => x.TotalAreaSquareMeters)
            .GreaterThan(0).WithMessage("TotalAreaSquareMeters must be greater than 0")
            .When(x => x.TotalAreaSquareMeters.HasValue);

        RuleFor(x => x.Precipitation)
            .GreaterThanOrEqualTo(0).WithMessage("Precipitation must be >= 0")
            .When(x => x.Precipitation.HasValue);
    }
}
