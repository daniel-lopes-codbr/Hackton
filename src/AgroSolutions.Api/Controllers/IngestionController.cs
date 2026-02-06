using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// High-performance ingestion controller for sensor data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class IngestionController : ControllerBase
{
    private readonly IIngestionService _ingestionService;
    private readonly ILogger<IngestionController> _logger;

    public IngestionController(IIngestionService ingestionService, ILogger<IngestionController> logger)
    {
        _ingestionService = ingestionService;
        _logger = logger;
    }

    /// <summary>
    /// Ingests a single sensor reading (Admin only)
    /// </summary>
    /// <param name="dto">Sensor reading data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created sensor reading</returns>
    [HttpPost("single")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SensorReadingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IngestSingle(
        [FromBody] SensorReadingDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
                var result = await _ingestionService.IngestSingleAsync(dto, cancellationToken);
                
                if (!result.IsSuccess)
                    return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });
                
                return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting single reading");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Ingests multiple sensor readings in batch (sequential processing) (Admin only)
    /// </summary>
    /// <param name="batchDto">Batch of sensor readings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Ingestion response with processing statistics</returns>
    [HttpPost("batch")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IngestionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IngestBatch(
        [FromBody] BatchSensorReadingDto batchDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
                var result = await _ingestionService.IngestBatchAsync(batchDto, cancellationToken);
                
                if (!result.IsSuccess)
                    return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });
                
                return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting batch");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Ingests multiple sensor readings in parallel for maximum performance (Admin only)
    /// </summary>
    /// <param name="batchDto">Batch of sensor readings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Ingestion response with processing statistics</returns>
    [HttpPost("batch/parallel")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IngestionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IngestBatchParallel(
        [FromBody] BatchSensorReadingDto batchDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
                var result = await _ingestionService.IngestBatchParallelAsync(batchDto, cancellationToken);
                
                if (!result.IsSuccess)
                    return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });
                
                return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting parallel batch");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a sensor reading by ID (Admin only)
    /// </summary>
    /// <param name="id">Sensor reading ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sensor reading</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SensorReadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var reading = await _ingestionService.GetByIdAsync(id, cancellationToken);
            
            if (reading == null)
                return NotFound(new { error = $"Sensor reading with ID {id} not found" });
            
            return Ok(reading);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sensor reading {ReadingId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all sensor readings for a specific field (Admin only)
    /// </summary>
    /// <param name="fieldId">Field ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sensor readings</returns>
    [HttpGet("field/{fieldId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFieldId(Guid fieldId, CancellationToken cancellationToken = default)
    {
        try
        {
            var readings = await _ingestionService.GetByFieldIdAsync(fieldId, cancellationToken);
            return Ok(readings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sensor readings for field {FieldId}", fieldId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get sensor readings for a field filtered by sensor type (Admin only)
    /// </summary>
    /// <param name="fieldId">Field ID</param>
    /// <param name="sensorType">Sensor type (e.g., Temperature, Humidity)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sensor readings</returns>
    [HttpGet("field/{fieldId}/sensor/{sensorType}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFieldIdAndSensorType(
        Guid fieldId, 
        string sensorType, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var readings = await _ingestionService.GetByFieldIdAsync(fieldId, cancellationToken);
            return Ok(readings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sensor readings for field {FieldId} and sensor type {SensorType}", fieldId, sensorType);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Health check endpoint for ingestion service
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "ingestion", timestamp = DateTime.UtcNow });
    }
}
