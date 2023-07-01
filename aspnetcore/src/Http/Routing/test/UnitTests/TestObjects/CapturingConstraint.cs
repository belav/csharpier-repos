using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.TestObjects;

internal class CapturingConstraint : IRouteConstraint
{
    public IDictionary<string, object> Values { get; private set; }

    public bool Match(
        HttpContext httpContext,
        IRouter route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        Values = new RouteValueDictionary(values);
        return true;
    }
}
