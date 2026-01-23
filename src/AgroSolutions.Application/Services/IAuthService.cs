using AgroSolutions.Application.Models;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service interface for Authentication
/// </summary>
public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<bool> VerifyPasswordAsync(string email, string password, CancellationToken cancellationToken = default);
}
