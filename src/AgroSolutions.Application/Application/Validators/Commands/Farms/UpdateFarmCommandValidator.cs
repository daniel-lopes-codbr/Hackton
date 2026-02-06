using AgroSolutions.Application.Commands.Farms;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Farms;

/// <summary>
/// Validator for UpdateFarmCommand
/// </summary>
public class UpdateFarmCommandValidator : AbstractValidator<UpdateFarmCommand>
{
    public UpdateFarmCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Farm ID is required");
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Farm name must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.WidthMeters)
            .GreaterThan(0).WithMessage("WidthMeters must be greater than 0")
            .When(x => x.WidthMeters.HasValue);

        RuleFor(x => x.LengthMeters)
            .GreaterThan(0).WithMessage("LengthMeters must be greater than 0")
            .When(x => x.LengthMeters.HasValue);

        RuleFor(x => x.TotalAreaSquareMeters)
            .GreaterThan(0).WithMessage("TotalAreaSquareMeters must be greater than 0")
            .When(x => x.TotalAreaSquareMeters.HasValue);

        RuleFor(x => x.Precipitation)
            .GreaterThanOrEqualTo(0).WithMessage("Precipitation must be >= 0")
            .When(x => x.Precipitation.HasValue);
    }
}
