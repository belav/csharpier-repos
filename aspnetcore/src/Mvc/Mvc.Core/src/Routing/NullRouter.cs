using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.Routing;

internal sealed class NullRouter : IRouter
{
    public static readonly IRouter Instance = new NullRouter();

    private NullRouter() { }

    public VirtualPathData? GetVirtualPath(VirtualPathContext context)
    {
        return null;
    }

    public Task RouteAsync(RouteContext context)
    {
        return Task.CompletedTask;
    }
}
