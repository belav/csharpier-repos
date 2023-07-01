using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupConfigureServicesThrows
{
    public void ConfigureServices(IServiceCollection services)
    {
        throw new Exception("Exception from ConfigureServices");
    }

    public void Configure(IApplicationBuilder builder) { }
}
