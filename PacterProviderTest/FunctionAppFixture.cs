using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;

namespace Pacter
{
    public class FunctionAppFixture : IDisposable
    {
        private readonly IHost server;
        public Uri ServerUri { get; }

        public FunctionAppFixture()
        {
            // Set the URI for the test server
            ServerUri = new Uri("http://localhost:9223");

            // Create the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .Build();

            // Create and start the test server
            server = Host.CreateDefaultBuilder()
                .ConfigureFunctionsWorkerDefaults(workerApp => 
                {
                    workerApp.UseMiddleware<ProviderStateMiddleware>();
                })
                .ConfigureAppConfiguration(config =>
                {
                    // Add the configuration directly in ConfigureAppConfiguration
                    config.AddConfiguration(configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    // No need to add IConfiguration again here
                    var startup = new TestStartup();
                    startup.ConfigureServices(services);
                })
                .Build();

            server.Start();
        }

        public void Dispose()
        {
            server?.Dispose();
        }
    }
}