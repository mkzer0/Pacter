using Microsoft.Extensions.DependencyInjection;
using Pacter;

namespace Pacter
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the fake repository for testing
            services.AddSingleton<IGreetingRepository, FakeGreetingRepository>();

            // Register the provider state middleware
            services.AddSingleton<ProviderStateMiddleware>();
        }
    }
}