using AgroSolutions.Api.Controllers;
using AgroSolutions.Application.Models;
using AgroSolutions.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Api.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UsersController>> _mockLogger;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_mockUserService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Users()
    {
        // Arrange
        var users = new List<UserDto> { new UserDto { Id = Guid.NewGuid(), Email = "a@a.com", Name = "A" } };
        _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<UserDto>>(ok.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_User_Missing()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockUserService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_Should_Return_Created_When_Success()
    {
        // Arrange
        var dto = new CreateUserDto { Name = "Test", Email = "t@t.com", Password = "p" };
        var created = new UserDto { Id = Guid.NewGuid(), Name = dto.Name, Email = dto.Email };
        var resultObj = AgroSolutions.Application.Common.Results.Result<UserDto>.Success(created);
        _mockUserService.Setup(s => s.CreateUserAsync(dto, It.IsAny<CancellationToken>())).Returns(Task.FromResult(resultObj));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<UserDto>(createdResult.Value);
        Assert.Equal(created.Email, value.Email);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var resultObj = AgroSolutions.Application.Common.Results.Result.Success();
        _mockUserService.Setup(s => s.DeleteUserAsync(id, It.IsAny<CancellationToken>())).Returns(Task.FromResult(resultObj));

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}

