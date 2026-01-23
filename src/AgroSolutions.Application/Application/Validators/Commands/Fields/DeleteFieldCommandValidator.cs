using AgroSolutions.Application.Commands.Fields;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Fields;

/// <summary>
/// Validator for DeleteFieldCommand
/// </summary>
public class DeleteFieldCommandValidator : AbstractValidator<DeleteFieldCommand>
{
    public DeleteFieldCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Field ID is required");
    }
}
