using AgroSolutions.Domain.Enums;

namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents an alert for agricultural monitoring
/// </summary>
public class Alert : Entity
{
    public Guid FieldId { get; private set; }
    public Guid FarmId { get; private set; }
    public AlertStatus Status { get; private set; }
    public string Message { get; private set; }
    public bool IsActive { get; private set; }

    private Alert() { } // For EF Core

    public Alert(Guid fieldId, Guid farmId, AlertStatus status, string message)
        : base()
    {
        if (fieldId == Guid.Empty)
            throw new ArgumentException("Field ID cannot be empty", nameof(fieldId));

        if (farmId == Guid.Empty)
            throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        FieldId = fieldId;
        FarmId = farmId;
        Status = status;
        Message = message;
        IsActive = true;
    }

    /// <summary>
    /// Deactivate the alert
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activate the alert
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    /// <summary>
    /// Update alert status and message
    /// </summary>
    public void UpdateStatus(AlertStatus status, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        Status = status;
        Message = message;
        MarkAsUpdated();
    }
}
