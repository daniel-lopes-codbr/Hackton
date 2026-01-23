using AgroSolutions.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AgroSolutions.Domain.Exceptions;
using AgroSolutions.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service implementation for Authentication
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", loginDto.Email);
                return null;
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password attempt for user: {Email}", loginDto.Email);
                return null;
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            _logger.LogInformation("User {Email} logged in successfully with role {Role}", user.Email, user.Role);

            return new TokenResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // Token expires in 24 hours
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
            throw new DomainException($"Failed to authenticate user: {ex.Message}", ex);
        }
    }

    public async Task<bool> VerifyPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    private string GenerateJwtToken(Domain.Entities.User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeInConfigurationAndAtLeast32CharactersLong";
        var issuer = jwtSettings["Issuer"] ?? "AgroSolutions";
        var audience = jwtSettings["Audience"] ?? "AgroSolutions";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "1440"); // 24 hours default

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
