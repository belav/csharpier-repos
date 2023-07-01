using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupTwoConfigures
{
    public StartupTwoConfigures() { }

    public void Configure(IApplicationBuilder builder) { }

    public void Configure(IApplicationBuilder builder, object service) { }
}
