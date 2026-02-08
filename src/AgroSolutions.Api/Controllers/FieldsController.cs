using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// Controller for managing fields
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // All endpoints require authentication
public class FieldsController : ControllerBase
{
    private readonly IFieldService _fieldService;
    private readonly ILogger<FieldsController> _logger;

    public FieldsController(IFieldService fieldService, ILogger<FieldsController> logger)
    {
        _fieldService = fieldService;
        _logger = logger;
    }

    /// <summary>
    /// Get all fields (User or Admin)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(IEnumerable<FieldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var fields = await _fieldService.GetAllAsync(cancellationToken);
        return Ok(fields);
    }

    /// <summary>
    /// Get fields by farm ID (User or Admin)
    /// </summary>
    [HttpGet("farm/{farmId}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(IEnumerable<FieldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFarmId(Guid farmId, CancellationToken cancellationToken = default)
    {
        var fields = await _fieldService.GetByFarmIdAsync(farmId, cancellationToken);
        return Ok(fields);
    }

    /// <summary>
    /// Get field by ID (User or Admin)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(FieldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var field = await _fieldService.GetByIdAsync(id, cancellationToken);
        if (field == null)
            return NotFound(new { error = $"Field with ID {id} not found" });

        return Ok(field);
    }

    /// <summary>
    /// Create a new field for a farm (User or Admin)
    /// </summary>
    [HttpPost("farm/{farmId}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(FieldDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(Guid farmId, [FromBody] CreateFieldDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _fieldService.CreateFieldAsync(farmId, dto, cancellationToken);
            
            if (!result.IsSuccess)
            {
                if (result.Errors.Any(e => e.Message.Contains("not found")))
                    return NotFound(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });
                
                return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });
            }
            
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating field for farm {FarmId}", farmId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing field (User or Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(FieldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFieldDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _fieldService.UpdateFieldAsync(id, dto, cancellationToken);
            
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
            _logger.LogError(ex, "Error updating field {FieldId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a field (User or Admin)
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
            var result = await _fieldService.DeleteFieldAsync(id, cancellationToken);
            
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
            _logger.LogError(ex, "Error deleting field {FieldId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
