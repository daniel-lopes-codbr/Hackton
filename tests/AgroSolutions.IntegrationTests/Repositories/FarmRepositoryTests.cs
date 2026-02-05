using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.ValueObjects;
using AgroSolutions.Infrastructure.Data;
using AgroSolutions.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AgroSolutions.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for FarmRepository
/// </summary>
public class FarmRepositoryTests : IntegrationTestBase
{
    private readonly IFarmRepository _repository;

    public FarmRepositoryTests()
    {
        Context.Database.EnsureCreated();
        _repository = new FarmRepository(Context);
    }

    [Fact]
    public async Task AddAsync_Should_Create_Farm()
    {
        // Arrange
        var property = new Property("Test Farm", "Test Location", 100.5m, "Test Description");
        var farm = new Farm(property, "Owner Name", "owner@example.com", "123456789");

        // Act
        await _repository.AddAsync(farm);
        await Context.SaveChangesAsync();

        // Assert
        var savedFarm = await Context.Set<Farm>().FirstOrDefaultAsync(f => f.Id == farm.Id);
        savedFarm.Should().NotBeNull();
        savedFarm!.Property.Name.Should().Be("Test Farm");
        savedFarm.OwnerName.Should().Be("Owner Name");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Farm_When_Exists()
    {
        // Arrange
        var property = new Property("Test Farm", "Location", 50m);
        var farm = new Farm(property, "Owner");
        await _repository.AddAsync(farm);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(farm.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Property.Name.Should().Be("Test Farm");
        result.OwnerName.Should().Be("Owner");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Farms()
    {
        // Arrange
        var farm1 = new Farm(new Property("Farm 1", "Location 1", 100m), "Owner 1");
        var farm2 = new Farm(new Property("Farm 2", "Location 2", 200m), "Owner 2");
        await _repository.AddAsync(farm1);
        await _repository.AddAsync(farm2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Property.Name == "Farm 1");
        result.Should().Contain(f => f.Property.Name == "Farm 2");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Farm()
    {
        // Arrange
        var property = new Property("Original Farm", "Location", 100m);
        var farm = new Farm(property, "Original Owner");
        await _repository.AddAsync(farm);
        await Context.SaveChangesAsync();

        // Act
        var newProperty = new Property("Updated Farm", "New Location", 150m);
        farm.UpdateProperty(newProperty);
        farm.UpdateOwnerInfo("Updated Owner", "new@example.com", "987654321");
        await _repository.UpdateAsync(farm);
        await Context.SaveChangesAsync();

        // Assert
        var updatedFarm = await _repository.GetByIdAsync(farm.Id);
        updatedFarm.Should().NotBeNull();
        updatedFarm!.Property.Name.Should().Be("Updated Farm");
        updatedFarm.OwnerName.Should().Be("Updated Owner");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Farm()
    {
        // Arrange
        var property = new Property("Test Farm", "Location", 100m);
        var farm = new Farm(property, "Owner");
        await _repository.AddAsync(farm);
        await Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(farm.Id);
        await Context.SaveChangesAsync();

        // Assert
        var deletedFarm = await _repository.GetByIdAsync(farm.Id);
        deletedFarm.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_Should_Return_Only_Farms_For_That_User()
    {
        // Arrange: create a user first (FK from Farm to User)
        var user = new User("Test User", "user@test.com", "hash", "Admin");
        Context.Set<User>().Add(user);
        await Context.SaveChangesAsync();

        var property1 = new Property("Farm User 1", "Location 1", 100m);
        var property2 = new Property("Farm Other", "Location 2", 200m);
        var farmForUser = new Farm(property1, "Owner 1", null, null, user.Id);
        var farmNoUser = new Farm(property2, "Owner 2"); // UserId = null

        await _repository.AddAsync(farmForUser);
        await _repository.AddAsync(farmNoUser);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(user.Id);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(f => f.Id == farmForUser.Id && f.Property.Name == "Farm User 1");
        result.Should().NotContain(f => f.Id == farmNoUser.Id);
    }
}
