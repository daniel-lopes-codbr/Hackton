using AgroSolutions.Domain.Enums;

namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents an alert aligned with diagram:
/// Id, FieldId, Status, IsEnable, CreatedAt
/// </summary>
public class Alert : Entity
{
    public Guid FieldId { get; private set; }
    public AlertStatus Status { get; private set; }
    public bool IsEnable { get; private set; }
    // Backwards compatibility
    public Guid? FarmId { get; private set; }
    public string? Message { get; private set; }
    public bool IsActive => IsEnable;

    private Alert() { } // For EF Core

    public Alert(Guid fieldId, AlertStatus status, bool isEnable = true)
        : base()
    {
        if (fieldId == Guid.Empty)
            throw new ArgumentException("Field ID cannot be empty", nameof(fieldId));

        FieldId = fieldId;
        Status = status;
        IsEnable = isEnable;
    }

    // Legacy constructor used in older codepaths
    public Alert(Guid fieldId, Guid farmId, AlertStatus status, string message)
        : this(fieldId, status, true)
    {
        FarmId = farmId;
        Message = message;
    }

    public void Disable()
    {
        IsEnable = false;
        MarkAsUpdated();
    }

    public void Enable()
    {
        IsEnable = true;
        MarkAsUpdated();
    }

    public void UpdateStatus(AlertStatus status)
    {
        Status = status;
        MarkAsUpdated();
    }
    // Legacy methods to preserve API
    public void Deactivate()
    {
        IsEnable = false;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsEnable = true;
        MarkAsUpdated();
    }
}
