using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Pacter;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(services =>
            {
                // Register the interface with its implementation
                services.AddSingleton<IGreetingRepository, GreetingRepository>();
            })
            .Build();

        host.Run();
    }
}