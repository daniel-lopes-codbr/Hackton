using AgroSolutions.Api.Controllers;
using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Invalid_Credentials()
    {
        // Arrange
        var dto = new LoginDto { Email = "no@one.com", Password = "pass" };
        _mockAuthService.Setup(s => s.LoginAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync((TokenResponseDto?)null);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_Should_Return_Ok_With_Token_When_Valid()
    {
        // Arrange
        var dto = new LoginDto { Email = "user@example.com", Password = "pass" };
        var token = new TokenResponseDto { Token = "abc", Email = dto.Email, Role = "User", ExpiresAt = DateTime.UtcNow.AddHours(1) };
        _mockAuthService.Setup(s => s.LoginAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(token);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TokenResponseDto>(ok.Value);
        Assert.Equal(token.Email, returned.Email);
    }
}

