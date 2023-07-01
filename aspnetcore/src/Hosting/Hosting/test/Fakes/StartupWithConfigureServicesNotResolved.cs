using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupWithConfigureServicesNotResolved
{
    public StartupWithConfigureServicesNotResolved() { }

    public void Configure(IApplicationBuilder builder, int notAService) { }
}
