using AgroSolutions.Api.Models;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service interface for Authentication
/// </summary>
public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<bool> VerifyPasswordAsync(string email, string password, CancellationToken cancellationToken = default);
}
