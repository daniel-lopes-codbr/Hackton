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
                
                return CreatedAtAction(nameof(IngestSingle), new { id = result.Value!.FieldId }, result.Value);
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
    /// Health check endpoint for ingestion service
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "ingestion", timestamp = DateTime.UtcNow });
    }
}
