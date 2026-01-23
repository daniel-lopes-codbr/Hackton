using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Queries.Users;

/// <summary>
/// Query to get a user by ID
/// </summary>
public class GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; set; }
}
