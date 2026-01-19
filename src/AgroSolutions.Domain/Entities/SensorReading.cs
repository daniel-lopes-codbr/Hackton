namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents a sensor reading from agricultural sensors
/// </summary>
public class SensorReading : Entity
{
    public Guid FieldId { get; private set; }
    public string SensorType { get; private set; } // Temperature, Humidity, SoilMoisture, etc.
    public decimal Value { get; private set; }
    public string Unit { get; private set; } // Celsius, Percent, etc.
    public DateTime ReadingTimestamp { get; private set; }
    public string? Location { get; private set; } // GPS coordinates or location identifier
    public Dictionary<string, string>? Metadata { get; private set; }

    private SensorReading() { } // For EF Core

    public SensorReading(
        Guid fieldId,
        string sensorType,
        decimal value,
        string unit,
        DateTime readingTimestamp,
        string? location = null,
        Dictionary<string, string>? metadata = null)
        : base()
    {
        if (fieldId == Guid.Empty)
            throw new ArgumentException("Field ID cannot be empty", nameof(fieldId));

        if (string.IsNullOrWhiteSpace(sensorType))
            throw new ArgumentException("Sensor type cannot be null or empty", nameof(sensorType));

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be null or empty", nameof(unit));

        FieldId = fieldId;
        SensorType = sensorType;
        Value = value;
        Unit = unit;
        ReadingTimestamp = readingTimestamp;
        Location = location;
        Metadata = metadata;
    }

    public void UpdateReading(decimal newValue, DateTime? newTimestamp = null)
    {
        Value = newValue;
        if (newTimestamp.HasValue)
        {
            ReadingTimestamp = newTimestamp.Value;
        }
        MarkAsUpdated();
    }
}
