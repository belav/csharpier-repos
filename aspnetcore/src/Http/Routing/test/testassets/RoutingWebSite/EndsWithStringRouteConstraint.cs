using System.Globalization;

namespace RoutingWebSite;

internal class EndsWithStringRouteConstraint : IRouteConstraint
{
    private readonly string _endsWith;

    public EndsWithStringRouteConstraint(string endsWith)
    {
        _endsWith = endsWith;
    }

    public bool Match(
        HttpContext httpContext,
        IRouter route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        var value = values[routeKey];
        if (value == null)
        {
            return false;
        }

        var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
        var endsWith = valueString.EndsWith(_endsWith, StringComparison.OrdinalIgnoreCase);
        return endsWith;
    }
}
