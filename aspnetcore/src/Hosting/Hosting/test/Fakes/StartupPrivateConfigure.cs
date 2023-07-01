using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupPrivateConfigure
{
    public StartupPrivateConfigure() { }

    public void ConfigureServices(IServiceCollection services) { }

    private void Configure(IApplicationBuilder builder) { }
}
