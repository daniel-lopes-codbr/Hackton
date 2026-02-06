using AgroSolutions.Domain.ValueObjects;

namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents a field (field plot) within a farm aligned with diagram:
/// Id, Name, FarmId, AreaSquareMeters, CropType
/// </summary>
public class Field : Entity
{
    public Guid FarmId { get; private set; }
    public string Name { get; private set; }
    public decimal AreaSquareMeters { get; private set; }
    public string CropType { get; private set; }
    // Backwards compatibility
    public Property? Property { get; private set; }
    public DateTime? PlantingDate { get; private set; }
    public DateTime? HarvestDate { get; private set; }

    private Field() { } // For EF Core

    public Field(Guid farmId, string name, decimal areaSquareMeters, string cropType)
        : base()
    {
        if (farmId == Guid.Empty)
            throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or empty", nameof(name));

        if (areaSquareMeters <= 0)
            throw new ArgumentException("Area must be positive", nameof(areaSquareMeters));

        if (string.IsNullOrWhiteSpace(cropType))
            throw new ArgumentException("Crop type cannot be null or empty", nameof(cropType));

        FarmId = farmId;
        Name = name;
        AreaSquareMeters = areaSquareMeters;
        CropType = cropType;
    }

    // Legacy constructor
    public Field(Guid farmId, Property property, string cropType, DateTime? plantingDate = null, DateTime? harvestDate = null)
        : base()
    {
        if (farmId == Guid.Empty)
            throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

        if (property == null) throw new ArgumentNullException(nameof(property));
        if (string.IsNullOrWhiteSpace(cropType))
            throw new ArgumentException("Crop type cannot be null or empty", nameof(cropType));

        FarmId = farmId;
        Property = property;
        Name = property.Name;
        AreaSquareMeters = property.Area;
        CropType = cropType;
        PlantingDate = plantingDate;
        HarvestDate = harvestDate;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Field name cannot be null or empty", nameof(name));
        Name = name;
        MarkAsUpdated();
    }

    public void UpdateArea(decimal areaSquareMeters)
    {
        if (areaSquareMeters <= 0) throw new ArgumentException("Area must be positive", nameof(areaSquareMeters));
        AreaSquareMeters = areaSquareMeters;
        MarkAsUpdated();
    }

    public void UpdateCropType(string cropType)
    {
        if (string.IsNullOrWhiteSpace(cropType)) throw new ArgumentException("Crop type cannot be null or empty", nameof(cropType));
        CropType = cropType;
        MarkAsUpdated();
    }
    // Legacy methods
    public void UpdateProperty(Property newProperty)
    {
        Property = newProperty ?? throw new ArgumentNullException(nameof(newProperty));
        Name = newProperty.Name;
        AreaSquareMeters = newProperty.Area;
        MarkAsUpdated();
    }

    public void UpdateCropInfo(string cropType, DateTime? plantingDate = null, DateTime? harvestDate = null)
    {
        UpdateCropType(cropType);
        PlantingDate = plantingDate;
        HarvestDate = harvestDate;
        MarkAsUpdated();
    }
}
