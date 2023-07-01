using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupWithILoggerFactory
{
    public ILoggerFactory ConstructorLoggerFactory { get; set; }

    public ILoggerFactory ConfigureLoggerFactory { get; set; }

    public StartupWithILoggerFactory(ILoggerFactory constructorLoggerFactory)
    {
        ConstructorLoggerFactory = constructorLoggerFactory;
    }

    public void ConfigureServices(IServiceCollection collection)
    {
        collection.AddSingleton(this);
    }

    public void Configure(IApplicationBuilder builder, ILoggerFactory loggerFactory)
    {
        ConfigureLoggerFactory = loggerFactory;
    }
}
