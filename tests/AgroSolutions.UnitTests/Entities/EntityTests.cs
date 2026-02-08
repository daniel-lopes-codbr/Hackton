using AgroSolutions.Domain.Entities;
using Xunit;

namespace AgroSolutions.Domain.Tests.Entities;

public class EntityTests
{
    [Fact]
    public void Entity_Should_Have_Unique_Id()
    {
        // Arrange & Act
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        // Assert
        Assert.NotEqual(entity1.Id, entity2.Id);
        Assert.NotEqual(Guid.Empty, entity1.Id);
        Assert.NotEqual(Guid.Empty, entity2.Id);
    }

    [Fact]
    public void Entity_Should_Have_CreatedAt_Timestamp()
    {
        // Arrange & Act
        var entity = new TestEntity();
        var now = DateTime.UtcNow;

        // Assert
        Assert.True(entity.CreatedAt <= now);
        Assert.True(entity.CreatedAt >= now.AddSeconds(-1));
    }

    [Fact]
    public void Entity_Should_Update_UpdatedAt_When_Marked_As_Updated()
    {
        // Arrange
        var entity = new TestEntity();
        var initialUpdatedAt = entity.UpdatedAt;

        // Act
        System.Threading.Thread.Sleep(10); // Small delay to ensure time difference
        entity.MarkAsUpdated();

        // Assert
        Assert.Null(initialUpdatedAt);
        Assert.NotNull(entity.UpdatedAt);
        Assert.True(entity.UpdatedAt > entity.CreatedAt);
    }

    [Fact]
    public void Entity_Equals_Should_Return_True_For_Same_Instance()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Assert.True(entity.Equals(entity));
    }

    [Fact]
    public void Entity_Equals_Should_Return_True_For_Same_Id()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.True(entity1.Equals(entity2));
        Assert.Equal(entity1, entity2);
    }

    [Fact]
    public void Entity_Equals_Should_Return_False_For_Different_Ids()
    {
        // Arrange
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
        Assert.NotEqual(entity1, entity2);
    }

    [Fact]
    public void Entity_GetHashCode_Should_Return_Id_HashCode()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Assert.Equal(entity.Id.GetHashCode(), entity.GetHashCode());
    }
}

// Test helper class
internal class TestEntity : Entity
{
    public TestEntity() : base() { }
    public TestEntity(Guid id) : base(id) { }
}

