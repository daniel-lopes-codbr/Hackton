using AgroSolutions.Application.Models;
using MediatR;

namespace AgroSolutions.Application.Queries.Users;

/// <summary>
/// Query to get all users
/// </summary>
public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
}
