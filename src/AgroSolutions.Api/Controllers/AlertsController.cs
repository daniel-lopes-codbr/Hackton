using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Api.Controllers;

/// <summary>
/// Controller for managing agricultural alerts
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // All endpoints require authentication
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(IAlertService alertService, ILogger<AlertsController> logger)
    {
        _alertService = alertService;
        _logger = logger;
    }

    /// <summary>
    /// Create alerts based on sensor readings from the last hour (Admin only)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Alert creation response with statistics</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AlertCreationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAlerts(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _alertService.CreateAlertsAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alerts");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update alerts - deactivates alerts created the previous day (Admin only)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of alerts deactivated</returns>
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateAlerts(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _alertService.UpdateAlertsAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors.Select(e => new { key = e.Key, message = e.Message }) });

            return Ok(new { deactivatedCount = result.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating alerts");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all alerts (Admin only)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all alerts</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _alertService.GetAllAsync(cancellationToken);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alerts");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get alert by ID (User or Admin)
    /// </summary>
    /// <param name="id">Alert ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Alert</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _alertService.GetByIdAsync(id, cancellationToken);

            if (alert == null)
                return NotFound(new { error = $"Alert with ID {id} not found" });

            return Ok(alert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alert {AlertId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get alerts by FieldId (User or Admin)
    /// </summary>
    /// <param name="fieldId">Field ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of alerts for the field</returns>
    [HttpGet("field/{fieldId}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFieldId(Guid fieldId, CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _alertService.GetByFieldIdAsync(fieldId, cancellationToken);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alerts for field {FieldId}", fieldId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get alerts by FarmId (User or Admin)
    /// </summary>
    /// <param name="farmId">Farm ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of alerts for the farm</returns>
    [HttpGet("farm/{farmId}")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFarmId(Guid farmId, CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _alertService.GetByFarmIdAsync(farmId, cancellationToken);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alerts for farm {FarmId}", farmId);
            return BadRequest(new { error = ex.Message });
        }
    }
}
