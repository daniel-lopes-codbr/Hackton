using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.ValueObjects;
using Xunit;

namespace AgroSolutions.Domain.Tests.Entities;

public class FieldTests
{
    [Fact]
    public void Field_Should_Create_With_Valid_Data()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property = new Property("Campo 1", "Fazenda São João", 50m);
        var cropType = "Soja";

        // Act
        var field = new Field(farmId, property, cropType);

        // Assert
        Assert.NotEqual(Guid.Empty, field.Id);
        Assert.Equal(farmId, field.FarmId);
        Assert.Equal(property, field.Property);
        Assert.Equal(cropType, field.CropType);
        Assert.Null(field.PlantingDate);
        Assert.Null(field.HarvestDate);
        Assert.NotEqual(default(DateTime), field.CreatedAt);
    }

    [Fact]
    public void Field_Should_Create_With_Planting_And_Harvest_Dates()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property = new Property("Campo 1", "Fazenda", 50m);
        var cropType = "Milho";
        var plantingDate = new DateTime(2024, 1, 15);
        var harvestDate = new DateTime(2024, 6, 20);

        // Act
        var field = new Field(farmId, property, cropType, plantingDate, harvestDate);

        // Assert
        Assert.Equal(plantingDate, field.PlantingDate);
        Assert.Equal(harvestDate, field.HarvestDate);
    }

    [Fact]
    public void Field_Should_Throw_Exception_When_FarmId_Is_Empty()
    {
        // Arrange
        var property = new Property("Campo", "Location", 50m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Field(Guid.Empty, property, "Soja"));
    }

    [Fact]
    public void Field_Should_Throw_Exception_When_CropType_Is_Null()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property = new Property("Campo", "Location", 50m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Field(farmId, property, null!));
    }

    [Fact]
    public void Field_Should_Throw_Exception_When_CropType_Is_Empty()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property = new Property("Campo", "Location", 50m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Field(farmId, property, ""));
    }

    [Fact]
    public void Field_Should_Throw_Exception_When_Property_Is_Null()
    {
        // Arrange
        var farmId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Field(farmId, null!, "Soja"));
    }

    [Fact]
    public void Field_Should_Update_Property()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property1 = new Property("Campo 1", "Location", 50m);
        var property2 = new Property("Campo 2", "Location", 75m);
        var field = new Field(farmId, property1, "Soja");
        var initialUpdatedAt = field.UpdatedAt;

        // Act
        System.Threading.Thread.Sleep(10);
        field.UpdateProperty(property2);

        // Assert
        Assert.Equal(property2, field.Property);
        Assert.NotNull(field.UpdatedAt);
        Assert.True(field.UpdatedAt > field.CreatedAt);
    }

    [Fact]
    public void Field_Should_Throw_Exception_When_Updating_Property_With_Null()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property = new Property("Campo", "Location", 50m);
        var field = new Field(farmId, property, "Soja");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => field.UpdateProperty(null!));
    }

    [Fact]
    public void Field_Should_Update_Crop_Info()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property = new Property("Campo", "Location", 50m);
        var field = new Field(farmId, property, "Soja");
        var initialUpdatedAt = field.UpdatedAt;
        var newPlantingDate = new DateTime(2024, 2, 1);
        var newHarvestDate = new DateTime(2024, 7, 1);

        // Act
        System.Threading.Thread.Sleep(10);
        field.UpdateCropInfo("Milho", newPlantingDate, newHarvestDate);

        // Assert
        Assert.Equal("Milho", field.CropType);
        Assert.Equal(newPlantingDate, field.PlantingDate);
        Assert.Equal(newHarvestDate, field.HarvestDate);
        Assert.NotNull(field.UpdatedAt);
        Assert.True(field.UpdatedAt > field.CreatedAt);
    }

    [Fact]
    public void Field_Should_Throw_Exception_When_Updating_CropType_With_Null()
    {
        // Arrange
        var farmId = Guid.NewGuid();
        var property = new Property("Campo", "Location", 50m);
        var field = new Field(farmId, property, "Soja");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => field.UpdateCropInfo(null!));
    }
}

