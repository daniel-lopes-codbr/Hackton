using AgroSolutions.Domain.Enums;

namespace AgroSolutions.Application.Models;

/// <summary>
/// DTO for Alert response
/// </summary>
public class AlertDto
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public Guid FarmId { get; set; }
    public AlertStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating alerts (triggers validation process)
/// </summary>
public class CreateAlertsDto
{
    // Empty - triggers automatic alert generation from sensor readings
}

/// <summary>
/// DTO for updating alerts (deactivate previous day alerts)
/// </summary>
public class UpdateAlertsDto
{
    // Empty - triggers deactivation of alerts from previous day
}

/// <summary>
/// Response DTO for alert creation process
/// </summary>
public class AlertCreationResponseDto
{
    public bool Success { get; set; }
    public int AlertsCreated { get; set; }
    public int FieldsProcessed { get; set; }
    public List<string>? Errors { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}
