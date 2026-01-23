using AgroSolutions.Api.Application.Commands.Users;
using FluentValidation;

namespace AgroSolutions.Api.Application.Validators.Commands.Users;

/// <summary>
/// Validator for DeleteUserCommand
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");
    }
}
