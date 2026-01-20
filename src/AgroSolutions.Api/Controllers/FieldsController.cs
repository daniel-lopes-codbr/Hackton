using AgroSolutions.Api.Models;
using AgroSolutions.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// Controller for managing fields
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// Get all fields
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FieldDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var fields = await _fieldService.GetAllAsync(cancellationToken);
        return Ok(fields);
    }

    /// <summary>
    /// Get fields by farm ID
    /// </summary>
    [HttpGet("farm/{farmId}")]
    [ProducesResponseType(typeof(IEnumerable<FieldDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByFarmId(Guid farmId, CancellationToken cancellationToken = default)
    {
        var fields = await _fieldService.GetByFarmIdAsync(farmId, cancellationToken);
        return Ok(fields);
    }

    /// <summary>
    /// Get field by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FieldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var field = await _fieldService.GetByIdAsync(id, cancellationToken);
        if (field == null)
            return NotFound(new { error = $"Field with ID {id} not found" });

        return Ok(field);
    }

    /// <summary>
    /// Create a new field for a farm
    /// </summary>
    [HttpPost("farm/{farmId}")]
    [ProducesResponseType(typeof(FieldDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid farmId, [FromBody] CreateFieldDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var field = await _fieldService.CreateAsync(farmId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = field.Id }, field);
        }
        catch (AgroSolutions.Domain.Exceptions.DomainException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating field for farm {FarmId}", farmId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing field
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FieldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFieldDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var field = await _fieldService.UpdateAsync(id, dto, cancellationToken);
            if (field == null)
                return NotFound(new { error = $"Field with ID {id} not found" });

            return Ok(field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating field {FieldId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a field
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _fieldService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(new { error = $"Field with ID {id} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting field {FieldId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
