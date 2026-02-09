using AgroSolutions.Domain.Entities;
using AgroSolutions.Functions.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AgroSolutions.Functions.Tests.Services;

public class AnalyticsServiceTests
{
    private readonly AnalyticsService _service;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsServiceTests()
    {
        _logger = new LoggerFactory().CreateLogger<AnalyticsService>();
        _service = new AnalyticsService(_logger);
    }

    [Fact]
    public async Task GetTrendAsync_Should_Return_Null_For_No_Readings()
    {
        var fieldId = Guid.NewGuid();
        var result = await _service.GetTrendAsync(fieldId, "Temperature");
        Assert.Null(result);
    }
}

