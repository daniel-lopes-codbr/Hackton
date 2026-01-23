using AgroSolutions.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Application.Commands.Users;

/// <summary>
/// Command to create a new user
/// </summary>
public class CreateUserCommand : IRequest<Result<Models.UserDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}
