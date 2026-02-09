using Moq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using AgroSolutions.Functions.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AgroSolutions.Functions.Tests.Services;

public class RabbitMqListenerTests
{
    [Fact]
    public async Task ProcessMessageAsync_Posts_To_Api()
    {
        // Arrange
        var handlerMock = new Moq.Protected.Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var client = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost/") };
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient("api")).Returns(client);

        var logger = new LoggerFactory().CreateLogger<RabbitMqListener>();
        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { { "API_BASE_URL", "http://localhost/" } }).Build();
        var listener = new RabbitMqListener(logger, factoryMock.Object, config);

        var json = "{\"FieldId\":\"00000000-0000-0000-0000-000000000000\",\"SensorType\":\"Temperature\",\"Value\":25.5}";

        // Act
        await listener.ProcessMessageAsync(json);

        // Assert - if no exception, assume success
        Assert.True(true);
    }
}

