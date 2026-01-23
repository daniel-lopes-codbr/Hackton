using AgroSolutions.Application.Commands.Farms;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Farms;

/// <summary>
/// Validator for DeleteFarmCommand
/// </summary>
public class DeleteFarmCommandValidator : AbstractValidator<DeleteFarmCommand>
{
    public DeleteFarmCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Farm ID is required");
    }
}
