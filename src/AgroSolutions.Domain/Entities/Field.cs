using AgroSolutions.Domain.ValueObjects;

namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents a field within a farm with identity and properties
/// </summary>
public class Field : Entity
{
    public Guid FarmId { get; private set; }
    public Property Property { get; private set; }
    public string CropType { get; private set; }
    public DateTime? PlantingDate { get; private set; }
    public DateTime? HarvestDate { get; private set; }

    private Field() { } // For EF Core

    public Field(Guid farmId, Property property, string cropType, DateTime? plantingDate = null, DateTime? harvestDate = null)
        : base()
    {
        if (farmId == Guid.Empty)
            throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

        if (string.IsNullOrWhiteSpace(cropType))
            throw new ArgumentException("Crop type cannot be null or empty", nameof(cropType));

        FarmId = farmId;
        Property = property ?? throw new ArgumentNullException(nameof(property));
        CropType = cropType;
        PlantingDate = plantingDate;
        HarvestDate = harvestDate;
    }

    public void UpdateProperty(Property newProperty)
    {
        Property = newProperty ?? throw new ArgumentNullException(nameof(newProperty));
        MarkAsUpdated();
    }

    public void UpdateCropInfo(string cropType, DateTime? plantingDate = null, DateTime? harvestDate = null)
    {
        if (string.IsNullOrWhiteSpace(cropType))
            throw new ArgumentException("Crop type cannot be null or empty", nameof(cropType));

        CropType = cropType;
        PlantingDate = plantingDate;
        HarvestDate = harvestDate;
        MarkAsUpdated();
    }
}
