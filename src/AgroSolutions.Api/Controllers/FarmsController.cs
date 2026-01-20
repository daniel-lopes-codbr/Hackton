using AgroSolutions.Api.Models;
using AgroSolutions.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// Controller for managing farms
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// Get all farms
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FarmDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var farms = await _farmService.GetAllAsync(cancellationToken);
        return Ok(farms);
    }

    /// <summary>
    /// Get farm by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var farm = await _farmService.GetByIdAsync(id, cancellationToken);
        if (farm == null)
            return NotFound(new { error = $"Farm with ID {id} not found" });

        return Ok(farm);
    }

    /// <summary>
    /// Create a new farm
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFarmDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var farm = await _farmService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = farm.Id }, farm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating farm");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing farm
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFarmDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var farm = await _farmService.UpdateAsync(id, dto, cancellationToken);
            if (farm == null)
                return NotFound(new { error = $"Farm with ID {id} not found" });

            return Ok(farm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating farm {FarmId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a farm
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _farmService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(new { error = $"Farm with ID {id} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting farm {FarmId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
