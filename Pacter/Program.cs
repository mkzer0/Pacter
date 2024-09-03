using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Pacter;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults() // Use this for .NET isolated worker
            .ConfigureServices(services =>
            {
                services.AddSingleton<GreetingRepository>(); // Register your repository
            })
            .Build();

        host.Run();
    }
}