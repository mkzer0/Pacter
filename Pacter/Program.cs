using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Pacter;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("FUNCTIONS_WORKER_GRPC_ADDRESS", "http://localhost:60528")
                });
            })
            .ConfigureServices(services =>
            {
                // Register the interface with its implementation
                services.AddSingleton<IGreetingRepository, GreetingRepository>();
            })
            .Build();

        host.Run();
    }
}