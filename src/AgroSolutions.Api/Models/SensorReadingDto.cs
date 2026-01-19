namespace AgroSolutions.Api.Models;

/// <summary>
/// DTO for single sensor reading ingestion
/// </summary>
public class SensorReadingDto
{
    public Guid FieldId { get; set; }
    public string SensorType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime ReadingTimestamp { get; set; }
    public string? Location { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// DTO for batch sensor readings ingestion
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
