namespace AgroSolutions.Api.Models;

/// <summary>
/// DTO for Field response
/// </summary>
public class FieldDto
{
    public Guid Id { get; set; }
    public Guid FarmId { get; set; }
    public PropertyDto Property { get; set; } = null!;
    public string CropType { get; set; } = string.Empty;
    public DateTime? PlantingDate { get; set; }
    public DateTime? HarvestDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Field
/// </summary>
public class CreateFieldDto
{
    public PropertyDto Property { get; set; } = null!;
    public string CropType { get; set; } = string.Empty;
    public DateTime? PlantingDate { get; set; }
    public DateTime? HarvestDate { get; set; }
}

/// <summary>
/// DTO for updating a Field
/// </summary>
public class UpdateFieldDto
{
    public PropertyDto? Property { get; set; }
    public string? CropType { get; set; }
    public DateTime? PlantingDate { get; set; }
    public DateTime? HarvestDate { get; set; }
}
