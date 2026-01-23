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
/// Integration tests for SensorReadingRepository
/// </summary>
public class SensorReadingRepositoryTests : IntegrationTestBase
{
    private readonly ISensorReadingRepository _repository;
    private readonly IFarmRepository _farmRepository;
    private readonly IFieldRepository _fieldRepository;

    public SensorReadingRepositoryTests()
    {
        _repository = new SensorReadingRepository(Context);
        _farmRepository = new FarmRepository(Context);
        _fieldRepository = new FieldRepository(Context);
    }

    [Fact]
    public async Task AddAsync_Should_Create_SensorReading()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Field", "Location", 50m), "Corn");
        await _fieldRepository.AddAsync(field);
        await Context.SaveChangesAsync();

        var reading = new SensorReading(
            field.Id,
            "Temperature",
            25.5m,
            "Celsius",
            DateTime.UtcNow,
            "GPS: -23.5505, -46.6333"
        );

        // Act
        await _repository.AddAsync(reading);
        await Context.SaveChangesAsync();

        // Assert
        var savedReading = await Context.Set<SensorReading>().FirstOrDefaultAsync(r => r.Id == reading.Id);
        savedReading.Should().NotBeNull();
        savedReading!.SensorType.Should().Be("Temperature");
        savedReading.Value.Should().Be(25.5m);
        savedReading.FieldId.Should().Be(field.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Reading_When_Exists()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Field", "Location", 50m), "Corn");
        await _fieldRepository.AddAsync(field);
        await Context.SaveChangesAsync();

        var reading = new SensorReading(field.Id, "Humidity", 60m, "Percent", DateTime.UtcNow);
        await _repository.AddAsync(reading);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(reading.Id);

        // Assert
        result.Should().NotBeNull();
        result!.SensorType.Should().Be("Humidity");
        result.Value.Should().Be(60m);
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
    public async Task GetByFieldIdAsync_Should_Return_Readings_For_Field()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field1 = new Field(farm.Id, new Property("Field 1", "Location", 50m), "Corn");
        var field2 = new Field(farm.Id, new Property("Field 2", "Location", 30m), "Wheat");
        await _fieldRepository.AddAsync(field1);
        await _fieldRepository.AddAsync(field2);
        await Context.SaveChangesAsync();

        var reading1 = new SensorReading(field1.Id, "Temperature", 25m, "Celsius", DateTime.UtcNow);
        var reading2 = new SensorReading(field1.Id, "Humidity", 60m, "Percent", DateTime.UtcNow);
        var reading3 = new SensorReading(field2.Id, "Temperature", 27m, "Celsius", DateTime.UtcNow);
        await _repository.AddAsync(reading1);
        await _repository.AddAsync(reading2);
        await _repository.AddAsync(reading3);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFieldIdAsync(field1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.SensorType == "Temperature" && r.Value == 25m);
        result.Should().Contain(r => r.SensorType == "Humidity" && r.Value == 60m);
        result.Should().NotContain(r => r.Value == 27m);
    }

    [Fact]
    public async Task GetByFieldIdAsync_Should_Return_Multiple_Readings()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Field", "Location", 50m), "Corn");
        await _fieldRepository.AddAsync(field);
        await Context.SaveChangesAsync();

        var reading1 = new SensorReading(field.Id, "Temperature", 25m, "Celsius", DateTime.UtcNow);
        var reading2 = new SensorReading(field.Id, "Humidity", 60m, "Percent", DateTime.UtcNow);
        await _repository.AddAsync(reading1);
        await _repository.AddAsync(reading2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFieldIdAsync(field.Id);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByFieldIdAndSensorTypeAsync_Should_Return_Filtered_Readings()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Field", "Location", 50m), "Corn");
        await _fieldRepository.AddAsync(field);
        await Context.SaveChangesAsync();

        var reading1 = new SensorReading(field.Id, "Temperature", 25m, "Celsius", DateTime.UtcNow);
        var reading2 = new SensorReading(field.Id, "Temperature", 26m, "Celsius", DateTime.UtcNow);
        var reading3 = new SensorReading(field.Id, "Humidity", 60m, "Percent", DateTime.UtcNow);
        await _repository.AddAsync(reading1);
        await _repository.AddAsync(reading2);
        await _repository.AddAsync(reading3);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFieldIdAndSensorTypeAsync(field.Id, "Temperature");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Value == 25m);
        result.Should().Contain(r => r.Value == 26m);
        result.Should().NotContain(r => r.SensorType == "Humidity");
    }

    [Fact]
    public async Task AddAsync_Should_Store_Metadata()
    {
        // Arrange
        var farm = new Farm(new Property("Farm", "Location", 100m), "Owner");
        await _farmRepository.AddAsync(farm);
        await Context.SaveChangesAsync();

        var field = new Field(farm.Id, new Property("Field", "Location", 50m), "Corn");
        await _fieldRepository.AddAsync(field);
        await Context.SaveChangesAsync();

        var metadata = new Dictionary<string, string>
        {
            { "SensorId", "SENSOR-001" },
            { "BatteryLevel", "85%" },
            { "SignalStrength", "Strong" }
        };

        var reading = new SensorReading(
            field.Id,
            "Temperature",
            25.5m,
            "Celsius",
            DateTime.UtcNow,
            null,
            metadata
        );

        // Act
        await _repository.AddAsync(reading);
        await Context.SaveChangesAsync();

        // Assert
        var savedReading = await _repository.GetByIdAsync(reading.Id);
        savedReading.Should().NotBeNull();
        savedReading!.Metadata.Should().NotBeNull();
        savedReading.Metadata!["SensorId"].Should().Be("SENSOR-001");
        savedReading.Metadata["BatteryLevel"].Should().Be("85%");
    }
}
