using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Exceptions;
using AgroSolutions.Domain.Repositories;
using BCrypt.Net;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service implementation for User management
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailAsync(email, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if email already exists
            if (await _repository.ExistsByEmailAsync(dto.Email, cancellationToken))
                throw new DomainException($"User with email {dto.Email} already exists");

            // Validate role
            if (dto.Role != "Admin" && dto.Role != "User")
                throw new DomainException("User role must be either 'Admin' or 'User'");

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User(
                dto.Name,
                dto.Email,
                passwordHash,
                dto.Role
            );

            await _repository.AddAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created user {UserId} with email {Email} and role {Role}", user.Id, user.Email, user.Role);

            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            throw new DomainException($"Failed to create user: {ex.Message}", ex);
        }
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return null;

            // Check email uniqueness if email is being updated
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _repository.ExistsByEmailAsync(dto.Email, cancellationToken))
                    throw new DomainException($"User with email {dto.Email} already exists");
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.UpdateName(dto.Name);

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.UpdateEmail(dto.Email);

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.UpdatePasswordHash(passwordHash);
            }

            if (!string.IsNullOrWhiteSpace(dto.Role))
                user.UpdateRole(dto.Role);

            await _repository.UpdateAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated user {UserId}", user.Id);

            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            throw new DomainException($"Failed to update user: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _repository.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                await _repository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Deleted user {UserId}", id);
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            throw new DomainException($"Failed to delete user: {ex.Message}", ex);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsByEmailAsync(email, cancellationToken);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
