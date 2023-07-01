using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupTwoConfigureServices
{
    public StartupTwoConfigureServices() { }

    public void ConfigureServices(IServiceCollection services) { }

    public void ConfigureServices(IServiceCollection services, object service) { }

    public void Configure(IApplicationBuilder builder) { }
}
