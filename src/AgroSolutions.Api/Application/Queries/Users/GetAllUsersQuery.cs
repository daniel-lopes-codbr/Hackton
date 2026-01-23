using AgroSolutions.Api.Models;
using MediatR;

namespace AgroSolutions.Api.Application.Queries.Users;

/// <summary>
/// Query to get all users
/// </summary>
public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
}
