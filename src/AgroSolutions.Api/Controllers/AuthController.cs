using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// Controller for authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login and get JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenResponse = await _authService.LoginAsync(loginDto, cancellationToken);
            if (tokenResponse == null)
                return Unauthorized(new { error = "Invalid email or password" });

            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
            return Unauthorized(new { error = "Authentication failed" });
        }
    }
}
