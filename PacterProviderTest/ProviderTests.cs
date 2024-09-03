using PactNet;
using PactNet.Verifier;
using System;
using System.Collections.Generic;
using System.IO;
using PactNet.Infrastructure.Outputters;
using NUnit.Framework;

namespace Pacter
{
    [TestFixture] // NUnit attribute for test classes
    public class ProviderTests
    {
        private FunctionAppFixture fixture;
        private PactVerifier pactVerifier;

        [SetUp] // Runs before each test
        public void SetUp()
        {
            fixture = new FunctionAppFixture();
        }

        [TearDown] // Runs after each test
        public void TearDown()
        {
            pactVerifier?.Dispose();
            fixture?.Dispose();
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

            string pactPath = Path.Combine("..", "..", "..", "..", "pacts", "GreetingConsumer-GreetingProvider.json");

            // Act / Assert
            pactVerifier = new PactVerifier(config);
            pactVerifier
                .ServiceProvider("GreetingProvider", fixture.ServerUri)
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(fixture.ServerUri, "/provider-states"))
                .Verify();
        }
    }
}