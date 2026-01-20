using AgroSolutions.Api.Models;
using AgroSolutions.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// Controller for managing users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")] // Only Admin can manage users
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound(new { error = $"User with ID {id} not found" });

        return Ok(user);
    }

    /// <summary>
    /// Create a new user (Admin only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing user (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, dto, cancellationToken);
            if (user == null)
                return NotFound(new { error = $"User with ID {id} not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a user (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _userService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(new { error = $"User with ID {id} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
