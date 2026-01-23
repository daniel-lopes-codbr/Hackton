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
/// Integration tests for FieldRepository
/// </summary>
public class FieldRepositoryTests : IntegrationTestBase
{
    private readonly IFieldRepository _repository;
    private readonly IFarmRepository _farmRepository;

    public FieldRepositoryTests()
    {
        _repository = new FieldRepository(Context);
        _farmRepository = new FarmRepository(Context);
    }

    [Fact]
    public async Task AddAsync_Should_Create_Field()
    {
        // Arrange
        var farm = new Farm(new Property("Test Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var property = new Property("Test Field", "Field Location", 50m);
        var field = new Field(farm.Id, property, "Corn", DateTime.UtcNow, null);

        // Act
        await _repository.AddAsync(field);
        await Context.SaveChangesAsync();

        // Assert
        var savedField = await Context.Set<Field>().FirstOrDefaultAsync(f => f.Id == field.Id);
        savedField.Should().NotBeNull();
        savedField!.Property.Name.Should().Be("Test Field");
        savedField.CropType.Should().Be("Corn");
        savedField.FarmId.Should().Be(farm.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Field_When_Exists()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Field", "Location", 50m), "Wheat");
        await _repository.AddAsync(field);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(field.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Property.Name.Should().Be("Field");
        result.CropType.Should().Be("Wheat");
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
    public async Task GetByFarmIdAsync_Should_Return_Fields_For_Farm()
    {
        // Arrange
        var farm1 = new Farm(new Property("Farm 1", "Location", 100m), "Owner 1");
        var farm2 = new Farm(new Property("Farm 2", "Location", 200m), "Owner 2");
        await _farmRepository.AddAsync(farm1);
        await _farmRepository.AddAsync(farm2);
        await Context.SaveChangesAsync();

        var field1 = new Field(farm1.Id, new Property("Field 1", "Location", 50m), "Corn");
        var field2 = new Field(farm1.Id, new Property("Field 2", "Location", 30m), "Wheat");
        var field3 = new Field(farm2.Id, new Property("Field 3", "Location", 40m), "Soy");
        await _repository.AddAsync(field1);
        await _repository.AddAsync(field2);
        await _repository.AddAsync(field3);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFarmIdAsync(farm1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Property.Name == "Field 1");
        result.Should().Contain(f => f.Property.Name == "Field 2");
        result.Should().NotContain(f => f.Property.Name == "Field 3");
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Fields()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field1 = new Field(farm.Id, new Property("Field 1", "Location", 50m), "Corn");
        var field2 = new Field(farm.Id, new Property("Field 2", "Location", 30m), "Wheat");
        await _repository.AddAsync(field1);
        await _repository.AddAsync(field2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Field()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Original Field", "Location", 50m), "Corn");
        await _repository.AddAsync(field);
        await Context.SaveChangesAsync();

        // Act
        var newProperty = new Property("Updated Field", "New Location", 60m);
        field.UpdateProperty(newProperty);
        field.UpdateCropInfo("Wheat", DateTime.UtcNow, DateTime.UtcNow.AddMonths(6));
        await _repository.UpdateAsync(field);
        await Context.SaveChangesAsync();

        // Assert
        var updatedField = await _repository.GetByIdAsync(field.Id);
        updatedField.Should().NotBeNull();
        updatedField!.Property.Name.Should().Be("Updated Field");
        updatedField.CropType.Should().Be("Wheat");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Field()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Field", "Location", 50m), "Corn");
        await _repository.AddAsync(field);
        await Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(field.Id);
        await Context.SaveChangesAsync();

        // Assert
        var deletedField = await _repository.GetByIdAsync(field.Id);
        deletedField.Should().BeNull();
    }
}
