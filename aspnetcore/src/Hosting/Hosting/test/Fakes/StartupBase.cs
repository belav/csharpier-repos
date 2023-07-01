using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupBase
{
    public void ConfigureBaseClassServices(IServiceCollection services)
    {
        services.AddOptions();
        services.Configure<FakeOptions>(o =>
        {
            o.Configured = true;
            o.Environment = "BaseClass";
        });
    }
}
