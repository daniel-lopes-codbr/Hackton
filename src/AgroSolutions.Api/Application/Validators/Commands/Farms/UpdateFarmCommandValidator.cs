using AgroSolutions.Api.Application.Commands.Farms;
using FluentValidation;

namespace AgroSolutions.Api.Application.Validators.Commands.Farms;

/// <summary>
/// Validator for UpdateFarmCommand
/// </summary>
public class UpdateFarmCommandValidator : AbstractValidator<UpdateFarmCommand>
{
    public UpdateFarmCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Farm ID is required");

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

        RuleFor(x => x.OwnerName)
            .MaximumLength(200).WithMessage("Owner name must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.OwnerName));

        RuleFor(x => x.OwnerEmail)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Owner email must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.OwnerEmail));
    }
}
