using AgroSolutions.Domain.ValueObjects;
using Xunit;

namespace AgroSolutions.Domain.Tests.ValueObjects;

public class PropertyTests
{
    [Fact]
    public void Property_Should_Create_With_Valid_Data()
    {
        // Arrange & Act
        var property = new Property("Fazenda São João", "São Paulo, SP", 100.5m, "Fazenda de soja");

        // Assert
        Assert.Equal("Fazenda São João", property.Name);
        Assert.Equal("São Paulo, SP", property.Location);
        Assert.Equal(100.5m, property.Area);
        Assert.Equal("Fazenda de soja", property.Description);
    }

    [Fact]
    public void Property_Should_Throw_Exception_When_Name_Is_Null()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Property(null!, "Location", 100m));
    }

    [Fact]
    public void Property_Should_Throw_Exception_When_Name_Is_Empty()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Property("", "Location", 100m));
    }

    [Fact]
    public void Property_Should_Throw_Exception_When_Name_Is_Whitespace()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Property("   ", "Location", 100m));
    }

    [Fact]
    public void Property_Should_Throw_Exception_When_Location_Is_Null()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Property("Name", null!, 100m));
    }

    [Fact]
    public void Property_Should_Throw_Exception_When_Location_Is_Empty()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Property("Name", "", 100m));
    }

    [Fact]
    public void Property_Should_Throw_Exception_When_Area_Is_Zero()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Property("Name", "Location", 0m));
    }

    [Fact]
    public void Property_Should_Throw_Exception_When_Area_Is_Negative()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Property("Name", "Location", -10m));
    }

    [Fact]
    public void Property_Should_Allow_Null_Description()
    {
        // Arrange & Act
        var property = new Property("Name", "Location", 100m, null);

        // Assert
        Assert.Null(property.Description);
    }

    [Fact]
    public void Property_Equals_Should_Return_True_For_Same_Values()
    {
        // Arrange
        var property1 = new Property("Fazenda A", "São Paulo", 100m);
        var property2 = new Property("Fazenda A", "São Paulo", 100m);

        // Act & Assert
        Assert.True(property1.Equals(property2));
        Assert.Equal(property1, property2);
    }

    [Fact]
    public void Property_Equals_Should_Return_False_For_Different_Values()
    {
        // Arrange
        var property1 = new Property("Fazenda A", "São Paulo", 100m);
        var property2 = new Property("Fazenda B", "São Paulo", 100m);

        // Act & Assert
        Assert.False(property1.Equals(property2));
        Assert.NotEqual(property1, property2);
    }

    [Fact]
    public void Property_ToString_Should_Return_Formatted_String()
    {
        // Arrange
        var property = new Property("Fazenda São João", "São Paulo, SP", 100.5m);

        // Act
        var result = property.ToString();

        // Assert
        Assert.Equal("Fazenda São João - São Paulo, SP (100.5 ha)", result);
    }
}
