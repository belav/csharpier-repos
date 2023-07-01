using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupNoServicesNoInterface
{
    public void ConfigureServices(IServiceCollection services) { }

    public void Configure(IApplicationBuilder app) { }
}
