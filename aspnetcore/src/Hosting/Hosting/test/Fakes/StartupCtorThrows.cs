using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupCtorThrows
{
    public StartupCtorThrows()
    {
        throw new Exception("Exception from constructor");
    }

    public void Configure(IApplicationBuilder app) { }
}
