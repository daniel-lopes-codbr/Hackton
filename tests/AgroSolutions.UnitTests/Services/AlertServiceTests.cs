using AgroSolutions.Application.Services;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;
using AgroSolutions.Application.Commands.Alerts;
using AgroSolutions.Application.Models;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgroSolutions.Application.Tests.Services;

public class AlertServiceTests
{
    [Fact]
    public async Task CreateAlertsAsync_Calls_Mediator()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var logger = new LoggerFactory().CreateLogger<AlertService>();

        var expected = AgroSolutions.Application.Common.Results.Result<AlertCreationResponseDto>.Success(new AlertCreationResponseDto { AlertsCreated = 0 });
        mockMediator.Setup(m => m.Send(It.IsAny<CreateAlertsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new AlertService(mockMediator.Object, mockMapper.Object);
        var result = await service.CreateAlertsAsync();

        mockMediator.Verify(m => m.Send(It.IsAny<CreateAlertsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateAlertsAsync_Calls_Mediator()
    {
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();
        var logger = new LoggerFactory().CreateLogger<AlertService>();

        var expected = AgroSolutions.Application.Common.Results.Result<int>.Success(5);
        mockMediator.Setup(m => m.Send(It.IsAny<UpdateAlertsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new AlertService(mockMediator.Object, mockMapper.Object);
        var result = await service.UpdateAlertsAsync();

        mockMediator.Verify(m => m.Send(It.IsAny<UpdateAlertsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result.IsSuccess);
    }
}

