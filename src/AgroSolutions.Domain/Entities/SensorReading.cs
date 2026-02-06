namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents telemetry aggregated reading aligned with diagram:
/// Id, FieldId, SoilMoisture, AirTemperature, Precipitation, IsRichInPests, CreatedAt
/// </summary>
public class SensorReading : Entity
{
    // Old fields (kept for backward compatibility with other services)
    public Guid FieldId { get; private set; }
    public string? SensorType { get; private set; } // Temperature, Humidity, SoilMoisture, etc.
    public decimal? Value { get; private set; }
    public string? Unit { get; private set; } // Celsius, Percent, etc.
    public DateTime? ReadingTimestamp { get; private set; }
    public string? Location { get; private set; } // GPS coordinates or location identifier
    public Dictionary<string, string>? Metadata { get; private set; }

    // New aggregated telemetry fields
    public decimal? SoilMoisture { get; private set; }
    public decimal? AirTemperature { get; private set; }
    public decimal? Precipitation { get; private set; }
    public bool? IsRichInPests { get; private set; }

    private SensorReading() { } // For EF Core

    // Original constructor kept for ingestion of single sensor-type readings
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

    // New constructor for aggregated telemetry ingestion
    public SensorReading(Guid fieldId, decimal? soilMoisture = null, decimal? airTemperature = null, decimal? precipitation = null, bool? isRichInPests = null)
        : base()
    {
        if (fieldId == Guid.Empty)
            throw new ArgumentException("Field ID cannot be empty", nameof(fieldId));

        FieldId = fieldId;
        SoilMoisture = soilMoisture;
        AirTemperature = airTemperature;
        Precipitation = precipitation;
        IsRichInPests = isRichInPests;
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

    public void UpdateTelemetry(decimal? soilMoisture = null, decimal? airTemperature = null, decimal? precipitation = null, bool? isRichInPests = null)
    {
        SoilMoisture = soilMoisture;
        AirTemperature = airTemperature;
        Precipitation = precipitation;
        IsRichInPests = isRichInPests;
        MarkAsUpdated();
    }
}
