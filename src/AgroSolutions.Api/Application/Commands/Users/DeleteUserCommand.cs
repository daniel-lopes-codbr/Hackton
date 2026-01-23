using AgroSolutions.Api.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Api.Application.Commands.Users;

/// <summary>
/// Command to delete a user
/// </summary>
public class DeleteUserCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
