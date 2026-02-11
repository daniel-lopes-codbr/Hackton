using AgroSolutions.Application.Services;
using AgroSolutions.Application.Models;
using AutoMapper;
using AgroSolutions.Application.Queries.Users;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Application.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_Calls_Mediator()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var mockRepo = new Mock<AgroSolutions.Domain.Repositories.IUserRepository>();
        var logger = new LoggerFactory().CreateLogger<UserService>();

        mockMediator.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserDto>());

        var service = new UserService(mockMediator.Object, mockMapper.Object, mockRepo.Object, logger);
        var result = await service.GetAllAsync();

        mockMediator.Verify(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByEmailAsync_Uses_Repository()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var mockRepo = new Mock<AgroSolutions.Domain.Repositories.IUserRepository>();
        var logger = new LoggerFactory().CreateLogger<UserService>();

        var user = new AgroSolutions.Domain.Entities.User("Name","a@a.com", BCrypt.Net.BCrypt.HashPassword("p"), "User");
        mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<AgroSolutions.Domain.Entities.User>())).Returns((AgroSolutions.Domain.Entities.User u) =>
            new UserDto { Id = u.Id, Name = u.Name, Email = u.Email });

        var service = new UserService(mockMediator.Object, mockMapper.Object, mockRepo.Object, logger);
        var dto = await service.GetByEmailAsync(user.Email);

        Assert.NotNull(dto);
        Assert.Equal(user.Email, dto.Email);
    }
}

