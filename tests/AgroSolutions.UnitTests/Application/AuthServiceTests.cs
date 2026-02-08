using AgroSolutions.Application.Services;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Application.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_Returns_Token_When_Credentials_Valid()
    {
        // Arrange
        var password = "mysecret";
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User("Name", "u@u.com", hashed, "User");
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var inMemory = new Dictionary<string, string?>
        {
            { "JwtSettings:SecretKey", "supersecretkeysupersecretkey1234" },
            { "JwtSettings:Issuer", "Agro" },
            { "JwtSettings:Audience", "Agro" },
            { "JwtSettings:ExpirationMinutes", "60" }
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
        var logger = new LoggerFactory().CreateLogger<AuthService>();

        var service = new AuthService(mockRepo.Object, configuration, logger);

        var dto = new LoginDto { Email = user.Email, Password = password };

        // Act
        var token = await service.LoginAsync(dto);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(user.Email, token.Email);
        Assert.False(string.IsNullOrWhiteSpace(token.Token));
    }

    [Fact]
    public async Task VerifyPasswordAsync_Returns_False_For_Wrong_Password()
    {
        // Arrange
        var password = "correct";
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User("Name", "a@a.com", hashed, "User");
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var logger = new LoggerFactory().CreateLogger<AuthService>();
        var service = new AuthService(mockRepo.Object, config, logger);

        // Act
        var ok = await service.VerifyPasswordAsync(user.Email, "badpassword");

        // Assert
        Assert.False(ok);
    }
}

