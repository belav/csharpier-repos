using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite;

public class RoutingController : Controller
{
    public bool HasEndpointMatch()
    {
        var endpointFeature = HttpContext.Features.Get<IEndpointFeature>();
        return endpointFeature?.Endpoint != null;
    }
}
