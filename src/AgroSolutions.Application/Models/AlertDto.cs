using AgroSolutions.Domain.Enums;

namespace AgroSolutions.Application.Models;

/// <summary>
/// DTO for Alert response aligned with diagram:
/// Id, FieldPlotId (FieldId), Status, IsEnable, CreatedAt
/// </summary>
public class AlertDto
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public AlertStatus Status { get; set; }
    public bool IsEnable { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating alerts (triggers validation process)
/// </summary>
// Note: alert generation/update commands are parameterless and use command objects.

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
