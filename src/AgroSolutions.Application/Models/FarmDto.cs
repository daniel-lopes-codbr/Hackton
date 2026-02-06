namespace AgroSolutions.Application.Models;

/// <summary>
/// DTO for Farm response matching diagram:
/// Id, Name, UserId, WidthMeters, LengthMeters, TotalAreaSquareMeters, Precipitation, CreatedAt, LastUpdatedAt
/// </summary>
public class FarmDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public decimal WidthMeters { get; set; }
    public decimal LengthMeters { get; set; }
    public decimal TotalAreaSquareMeters { get; set; }
    public decimal? Precipitation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Farm
/// </summary>
public class CreateFarmDto
{
    public string Name { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public decimal WidthMeters { get; set; }
    public decimal LengthMeters { get; set; }
    public decimal? TotalAreaSquareMeters { get; set; }
    public decimal? Precipitation { get; set; }
}

/// <summary>
/// DTO for updating a Farm
/// </summary>
public class UpdateFarmDto
{
    public string? Name { get; set; }
    public Guid? UserId { get; set; }
    public decimal? WidthMeters { get; set; }
    public decimal? LengthMeters { get; set; }
    public decimal? TotalAreaSquareMeters { get; set; }
    public decimal? Precipitation { get; set; }
}
