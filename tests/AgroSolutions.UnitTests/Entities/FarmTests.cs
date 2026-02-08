using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.ValueObjects;
using Xunit;

namespace AgroSolutions.Domain.Tests.Entities;

public class FarmTests
{
    [Fact]
    public void Farm_Should_Create_With_Valid_Data()
    {
        // Arrange
        var property = new Property("Fazenda São João", "São Paulo, SP", 100.5m);
        var ownerName = "João Silva";

        // Act
        var farm = new Farm(property, ownerName);

        // Assert
        Assert.NotEqual(Guid.Empty, farm.Id);
        Assert.Equal(property, farm.Property);
        Assert.Equal(ownerName, farm.OwnerName);
        Assert.Null(farm.OwnerEmail);
        Assert.Null(farm.OwnerPhone);
        Assert.NotEqual(default(DateTime), farm.CreatedAt);
    }

    [Fact]
    public void Farm_Should_Create_With_All_Owner_Information()
    {
        // Arrange
        var property = new Property("Fazenda São João", "São Paulo, SP", 100.5m);
        var ownerName = "João Silva";
        var ownerEmail = "joao@example.com";
        var ownerPhone = "+55 11 99999-9999";

        // Act
        var farm = new Farm(property, ownerName, ownerEmail, ownerPhone);

        // Assert
        Assert.Equal(ownerName, farm.OwnerName);
        Assert.Equal(ownerEmail, farm.OwnerEmail);
        Assert.Equal(ownerPhone, farm.OwnerPhone);
    }

    [Fact]
    public void Farm_Should_Create_With_UserId()
    {
        // Arrange
        var property = new Property("Fazenda", "Location", 100m);
        var userId = Guid.NewGuid();

        // Act
        var farm = new Farm(property, "Owner", null, null, userId);

        // Assert
        Assert.Equal(userId, farm.UserId);
    }

    [Fact]
    public void Farm_Should_Throw_Exception_When_OwnerName_Is_Null()
    {
        // Arrange
        var property = new Property("Fazenda", "Location", 100m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Farm(property, null!));
    }

    [Fact]
    public void Farm_Should_Throw_Exception_When_OwnerName_Is_Empty()
    {
        // Arrange
        var property = new Property("Fazenda", "Location", 100m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Farm(property, ""));
    }

    [Fact]
    public void Farm_Should_Throw_Exception_When_Property_Is_Null()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Farm(null!, "Owner Name"));
    }

    [Fact]
    public void Farm_Should_Update_Property()
    {
        // Arrange
        var property1 = new Property("Fazenda A", "São Paulo", 100m);
        var property2 = new Property("Fazenda B", "Rio de Janeiro", 200m);
        var farm = new Farm(property1, "João Silva");
        var initialUpdatedAt = farm.UpdatedAt;

        // Act
        System.Threading.Thread.Sleep(10);
        farm.UpdateProperty(property2);

        // Assert
        Assert.Equal(property2, farm.Property);
        Assert.NotNull(farm.UpdatedAt);
        Assert.True(farm.UpdatedAt!.Value > farm.CreatedAt);
    }

    [Fact]
    public void Farm_Should_Throw_Exception_When_Updating_Property_With_Null()
    {
        // Arrange
        var property = new Property("Fazenda", "Location", 100m);
        var farm = new Farm(property, "João Silva");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => farm.UpdateProperty(null!));
    }

    [Fact]
    public void Farm_Should_Update_Owner_Info()
    {
        // Arrange
        var property = new Property("Fazenda", "Location", 100m);
        var farm = new Farm(property, "João Silva");
        var initialUpdatedAt = farm.UpdatedAt;

        // Act
        System.Threading.Thread.Sleep(10);
        farm.UpdateOwnerInfo("Maria Silva", "maria@example.com", "+55 11 88888-8888");

        // Assert
        Assert.Equal("Maria Silva", farm.OwnerName);
        Assert.Equal("maria@example.com", farm.OwnerEmail);
        Assert.Equal("+55 11 88888-8888", farm.OwnerPhone);
        Assert.NotNull(farm.UpdatedAt);
        Assert.True(farm.UpdatedAt!.Value > farm.CreatedAt);
    }

    [Fact]
    public void Farm_Should_Throw_Exception_When_Updating_OwnerName_With_Null()
    {
        // Arrange
        var property = new Property("Fazenda", "Location", 100m);
        var farm = new Farm(property, "João Silva");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => farm.UpdateOwnerInfo(null!));
    }
}

