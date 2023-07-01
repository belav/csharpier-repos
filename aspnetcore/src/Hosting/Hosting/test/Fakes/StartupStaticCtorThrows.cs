using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class StartupStaticCtorThrows
{
    static StartupStaticCtorThrows()
    {
        throw new Exception("Exception from static constructor");
    }

    public void Configure(IApplicationBuilder app) { }
}
