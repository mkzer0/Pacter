using PactNet;
using PactNet.Verifier;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PactNet.Infrastructure.Outputters;
using NUnit.Framework;

namespace Pacter
{
    [TestFixture] // NUnit attribute for test classes
    public class ProviderTests
    {
        //private FunctionAppFixture fixture;
        private PactVerifier pactVerifier;
        private IHost _host;
        private HttpClient _httpClient;

        [SetUp] // Runs before each test
        public void Setup()
        {
            _host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker =>
                    {
                        worker.UseMiddleware<ProviderStateMiddleware>();
                    }
                    )// This sets up the function app for isolated worker model
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // Add custom configuration settings from example.settings.json
                    var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    config.SetBasePath(location)
                        .AddJsonFile("test.settings.json", optional: false, reloadOnChange: true)
                        .AddConfiguration(hostContext.Configuration); // Ensure the current host configuration is added as well
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IGreetingRepository, FakeGreetingRepository>();
                })
                .Build();

             _host.RunAsync();

            // Setup HttpClient to communicate with the running function app
            _httpClient = new HttpClient { BaseAddress = new System.Uri("http://localhost:7074") };
        }

        [TearDown] // Runs after each test
        public void TearDown()
        {
            pactVerifier?.Dispose();
            _host?.Dispose();
        }

        [Test] // NUnit attribute for test methods
        public void EnsureFunctionAppHonoursPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                /*Outputters = new List<IOutput>
                {
                    new NUnitOutput(TestContext.Out), // Using NUnitOutput to handle test output
                },*/
                LogLevel = PactLogLevel.Debug
            };
            var result = _httpClient.GetAsync("http://localhost:7072/welcome");
            result.Result.EnsureSuccessStatusCode();
            string pactPath = Path.Combine("..", "..", "..", "..", "pacts", "PacterConsumer-PacterProvider.json");

            // Act / Assert
            pactVerifier = new PactVerifier("Pacter",config);
            pactVerifier
                .WithHttpEndpoint(new Uri("http://localhost:7074"))
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri("http://localhost:7074"+ "/provider-states"))
                .Verify();
        }
    }
}