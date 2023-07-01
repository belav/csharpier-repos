using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupNoServices : Hosting.StartupBase
{
    public StartupNoServices() { }

    public override void Configure(IApplicationBuilder builder) { }
}
