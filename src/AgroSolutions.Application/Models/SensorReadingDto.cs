namespace AgroSolutions.Application.Models;

/// <summary>
/// DTO for telemetry ingestion (aligned with diagram)
/// Id, FieldId (FieldPlotId), SoilMoisture, AirTemperature, Precipitation, IsRichInPests, CreatedAt
/// </summary>
public partial class SensorReadingDto
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public decimal? SoilMoisture { get; set; }
    public decimal? AirTemperature { get; set; }
    public decimal? Precipitation { get; set; }
    public bool? IsRichInPests { get; set; }
    public DateTime CreatedAt { get; set; }
}
// Backwards-compatible legacy properties for single-sensor readings
public partial class SensorReadingDto
{
    public string? SensorType { get; set; }
    public decimal? Value { get; set; }
    public string? Unit { get; set; }
    public DateTime? ReadingTimestamp { get; set; }
    public string? Location { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// DTO for batch telemetry ingestion
/// </summary>
public class BatchSensorReadingDto
{
    public List<SensorReadingDto> Readings { get; set; } = new();
}

/// <summary>
/// Response DTO for ingestion operations
/// </summary>
public class IngestionResponseDto
{
    public bool Success { get; set; }
    public int ProcessedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string>? Errors { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}
