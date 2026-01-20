namespace AgroSolutions.Api.Models;

/// <summary>
/// DTO for Farm response
/// </summary>
public class FarmDto
{
    public Guid Id { get; set; }
    public PropertyDto Property { get; set; } = null!;
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Farm
/// </summary>
public class CreateFarmDto
{
    public PropertyDto Property { get; set; } = null!;
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
}

/// <summary>
/// DTO for updating a Farm
/// </summary>
public class UpdateFarmDto
{
    public PropertyDto? Property { get; set; }
    public string? OwnerName { get; set; }
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
}

/// <summary>
/// DTO for Property (Value Object)
/// </summary>
public class PropertyDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public string? Description { get; set; }
}
