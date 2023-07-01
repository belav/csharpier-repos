using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.Constraints;

internal sealed class NullRouteConstraint : IRouteConstraint
{
    public static readonly NullRouteConstraint Instance = new NullRouteConstraint();

    private NullRouteConstraint() { }

    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        return true;
    }
}
