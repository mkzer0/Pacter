using NUnit.Framework;
using PactNet;
using PactNet.Matchers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

[TestFixture]
public class PacterConsumerTest
{
    private IPactBuilderV4 pact;
    private Mock<IHttpClientFactory> mockFactory;

    [SetUp]
    public void SetUp()
    {
        this.mockFactory = new Mock<IHttpClientFactory>();

        var config = new PactConfig
        {
            PactDir = "../../../../pacts/",
            DefaultJsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            },
            LogLevel = PactLogLevel.Debug
        };

        this.pact = Pact.V4("PacterConsumer", "PacterProvider", config).WithHttpInteractions();
    }

    [Test]
    public async Task ItHandlesGreetingRequestCorrectly()
    {
        // Arrange - Define the interaction
        this.pact
            .UponReceiving("A GET request to /welcome")
            .Given("A greeting is available")
            .WithRequest(HttpMethod.Get, "/welcome")
            .WillRespond()
            .WithStatus(200)
            .WithHeader("Content-Type", "text/plain; charset=utf-8")
            .WithBody("Hello, Pact!", "text/plain");

        await this.pact.VerifyAsync(async ctx =>
        {
            // Set up the mock IHttpClientFactory to return a new HttpClient with the mock server's URI
            this.mockFactory
                .Setup(f => f.CreateClient("Pacter"))
                .Returns(() => new HttpClient
                {
                    BaseAddress = ctx.MockServerUri,
                    DefaultRequestHeaders =
                    {
                        Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }
                    }
                });

            // Use the mocked IHttpClientFactory to create PacterConsumer
            var client = new PacterConsumer(this.mockFactory.Object);

            // Act
            string greeting = await client.GetGreetingAsync();

            // Assert
            greeting.Should().Be("Hello, Pact!");
        });
    }

    [TearDown]
    public void TearDown()
    {
        // No resources to dispose since HttpClient is mocked
    }
}