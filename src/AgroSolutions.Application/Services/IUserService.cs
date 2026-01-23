using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service interface for User management
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
