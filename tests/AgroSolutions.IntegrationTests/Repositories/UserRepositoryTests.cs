using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Infrastructure.Data;
using AgroSolutions.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AgroSolutions.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for UserRepository
/// </summary>
public class UserRepositoryTests : IntegrationTestBase
{
    private readonly IUserRepository _repository;

    public UserRepositoryTests()
    {
        _repository = new UserRepository(Context);
    }

    [Fact]
    public async Task AddAsync_Should_Create_User()
    {
        // Arrange
        var user = new User(
            "Test User",
            "test@example.com",
            "hashedpassword",
            "User"
        );

        // Act
        await _repository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Assert
        var savedUser = await Context.Set<User>().FirstOrDefaultAsync(u => u.Id == user.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Name.Should().Be("Test User");
        savedUser.Email.Should().Be("test@example.com");
        savedUser.Role.Should().Be("User");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_When_Exists()
    {
        // Arrange
        var user = new User(
            "Test User",
            "test@example.com",
            "hashedpassword",
            "Admin"
        );
        await _repository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test User");
        result.Email.Should().Be("test@example.com");
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
    public async Task GetByEmailAsync_Should_Return_User_When_Exists()
    {
        // Arrange
        var email = "test@example.com";
        var user = new User(
            "Test User",
            email,
            "hashedpassword",
            "User"
        );
        await _repository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Users()
    {
        // Arrange
        var user1 = new User("User 1", "user1@example.com", "hash1", "User");
        var user2 = new User("User 2", "user2@example.com", "hash2", "Admin");
        await _repository.AddAsync(user1);
        await _repository.AddAsync(user2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "user1@example.com");
        result.Should().Contain(u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_User()
    {
        // Arrange
        var user = new User("Original Name", "original@example.com", "hash", "User");
        await _repository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Act
        user.UpdateName("Updated Name");
        user.UpdateEmail("updated@example.com");
        await _repository.UpdateAsync(user);
        await Context.SaveChangesAsync();

        // Assert
        var updatedUser = await _repository.GetByIdAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be("Updated Name");
        updatedUser.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_User()
    {
        // Arrange
        var user = new User("Test User", "test@example.com", "hash", "User");
        await _repository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(user.Id);
        await Context.SaveChangesAsync();

        // Assert
        var deletedUser = await _repository.GetByIdAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task EmailExistsAsync_Should_Return_True_When_Email_Exists()
    {
        // Arrange
        var email = "existing@example.com";
        var user = new User("Test User", email, "hash", "User");
        await _repository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByEmailAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_Should_Return_False_When_Email_Not_Exists()
    {
        // Arrange
        var email = "nonexistent@example.com";

        // Act
        var result = await _repository.ExistsByEmailAsync(email);

        // Assert
        result.Should().BeFalse();
    }
}
