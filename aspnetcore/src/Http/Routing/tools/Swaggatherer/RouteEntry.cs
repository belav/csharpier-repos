using Microsoft.AspNetCore.Routing.Template;

namespace Swaggatherer;

internal sealed class RouteEntry
{
    public RouteTemplate Template { get; set; }
    public string Method { get; set; }
    public decimal Precedence { get; set; }
    public string RequestUrl { get; set; }
}
