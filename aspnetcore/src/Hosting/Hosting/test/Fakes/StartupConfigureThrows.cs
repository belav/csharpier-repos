using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupConfigureThrows
{
    public void ConfigureServices(IServiceCollection services) { }

    public void Configure(IApplicationBuilder builder)
    {
        throw new Exception("Exception from Configure");
    }
}
