using AgroSolutions.Application.Services;
using AgroSolutions.Application.Models;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Commands.Farms;
using AutoMapper;
using AgroSolutions.Application.Queries.Farms;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Application.Tests.Services;

public class FarmServiceTests
{
    [Fact]
    public async Task GetAllAsync_Invokes_Mediator()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var mockRepo = new Mock<AgroSolutions.Domain.Repositories.IFarmRepository>();
        var logger = new LoggerFactory().CreateLogger<FarmService>();

        mockMediator.Setup(m => m.Send(It.IsAny<GetAllFarmsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FarmDto>());

        var service = new FarmService(mockMediator.Object, mockMapper.Object, mockRepo.Object, logger);

        var result = await service.GetAllAsync();

        mockMediator.Verify(m => m.Send(It.IsAny<GetAllFarmsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ExistsAsync_Uses_Repository()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var mockRepo = new Mock<AgroSolutions.Domain.Repositories.IFarmRepository>();
        var logger = new LoggerFactory().CreateLogger<FarmService>();

        mockRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new FarmService(mockMediator.Object, mockMapper.Object, mockRepo.Object, logger);

        var exists = await service.ExistsAsync(Guid.NewGuid());

        Assert.True(exists);
    }
}

