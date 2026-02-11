using AgroSolutions.Application.Services;
using AgroSolutions.Application.Models;
using AutoMapper;
using AgroSolutions.Application.Queries.Fields;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Application.Tests.Services;

public class FieldServiceTests
{
    [Fact]
    public async Task GetAllAsync_Calls_Mediator()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var mockRepo = new Mock<AgroSolutions.Domain.Repositories.IFieldRepository>();
        var logger = new LoggerFactory().CreateLogger<FieldService>();

        mockMediator.Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<FieldDto>());

        var service = new FieldService(mockMediator.Object, mockMapper.Object, mockRepo.Object, logger);
        var result = await service.GetAllAsync();

        mockMediator.Verify(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ExistsAsync_Uses_Repository()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var mockRepo = new Mock<AgroSolutions.Domain.Repositories.IFieldRepository>();
        var logger = new LoggerFactory().CreateLogger<FieldService>();

        mockRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new FieldService(mockMediator.Object, mockMapper.Object, mockRepo.Object, logger);
        var exists = await service.ExistsAsync(Guid.NewGuid());

        Assert.True(exists);
    }
}

