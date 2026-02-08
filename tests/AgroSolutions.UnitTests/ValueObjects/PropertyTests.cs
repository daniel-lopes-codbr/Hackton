using AgroSolutions.Domain.ValueObjects;
using Xunit;

namespace AgroSolutions.Domain.Tests.ValueObjects;

public class PropertyTests
{
    [Fact]
    public void Property_Should_Create_With_Valid_Data()
    {
        // Arrange & Act
        var prop = new Property("Fazenda A", "Local", 150m);

        // Assert
        Assert.Equal("Fazenda A", prop.Name);
        Assert.Equal("Local", prop.Location);
        Assert.Equal(150m, prop.Area);
    }

    [Fact]
    public void Property_Equals_Should_Consider_All_Fields()
    {
        // Arrange
        var p1 = new Property("A", "X", 10m);
        var p2 = new Property("A", "X", 10m);

        // Act & Assert
        Assert.Equal(p1, p2);
        Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
    }
}

