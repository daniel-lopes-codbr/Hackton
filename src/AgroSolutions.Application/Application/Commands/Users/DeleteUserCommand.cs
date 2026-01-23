using AgroSolutions.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Application.Commands.Users;

/// <summary>
/// Command to delete a user
/// </summary>
public class DeleteUserCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
