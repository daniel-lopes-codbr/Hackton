using AgroSolutions.Api.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Api.Application.Commands.Users;

/// <summary>
/// Command to update an existing user
/// </summary>
public class UpdateUserCommand : IRequest<Result<Models.UserDto>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}
