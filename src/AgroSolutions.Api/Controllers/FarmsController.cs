using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// Controller for managing farms
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // All endpoints require authentication
public class FarmsController : ControllerBase
{
    private readonly IFarmService _farmService;
    private readonly ILogger<FarmsController> _logger;

    public FarmsController(IFarmService farmService, ILogger<FarmsController> logger)
    {
        _farmService = farmService;
        _logger = logger;
    }

    /// <summary>
    /// Get all farms (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<FarmDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var farms = await _farmService.GetAllAsync(cancellationToken);
        return Ok(farms);
    }

    /// <summary>
    /// Get all farms for a specific user (User or Admin)
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(IEnumerable<FarmDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var farms = await _farmService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(farms);
    }

    /// <summary>
    /// Get farm by ID (User or Admin)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var farm = await _farmService.GetByIdAsync(id, cancellationToken);
        if (farm == null)
            return NotFound(new { error = $"Farm with ID {id} not found" });

        return Ok(farm);
    }

    /// <summary>
    /// Create a new farm (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateFarmDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _farmService.CreateFarmAsync(dto, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error creating farm - database update failed");
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new
            {
                error = "Erro ao salvar a fazenda. Se o banco foi criado antes de adicionar a coluna UserId, exclua o arquivo AgroSolutions.db (ou rode 'dotnet ef database update') e reinicie a API.",
                detail = innerMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating farm");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing farm (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFarmDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _farmService.UpdateFarmAsync(id, dto, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Errors.Any(e => e.Message.Contains("not found")))
                    return NotFound(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });

                return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating farm {FarmId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a farm (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _farmService.DeleteFarmAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Errors.Any(e => e.Message.Contains("not found")))
                    return NotFound(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });

                return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting farm {FarmId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
