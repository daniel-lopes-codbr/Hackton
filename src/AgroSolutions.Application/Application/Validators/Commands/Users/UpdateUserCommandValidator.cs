using AgroSolutions.Application.Commands.Users;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Users;

/// <summary>
/// Validator for UpdateUserCommand
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));

        RuleFor(x => x.Role)
            .Must(r => r == "Admin" || r == "User").WithMessage("Role must be either 'Admin' or 'User'")
            .When(x => !string.IsNullOrWhiteSpace(x.Role));
    }
}
