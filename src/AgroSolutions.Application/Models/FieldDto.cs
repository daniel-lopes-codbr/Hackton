namespace AgroSolutions.Application.Models;

/// <summary>
/// DTO for Field (FieldPlot) response matching diagram:
/// Id, Name, FarmId, AreaSquareMeters, CropType, CreatedAt, LastUpdatedAt
/// </summary>
public class FieldDto
{
    public Guid Id { get; set; }
    public Guid FarmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal AreaSquareMeters { get; set; }
    public string CropType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Field
/// </summary>
public class CreateFieldDto
{
    public string Name { get; set; } = string.Empty;
    public decimal AreaSquareMeters { get; set; }
    public string CropType { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating a Field
/// </summary>
public class UpdateFieldDto
{
    public string? Name { get; set; }
    public decimal? AreaSquareMeters { get; set; }
    public string? CropType { get; set; }
}
