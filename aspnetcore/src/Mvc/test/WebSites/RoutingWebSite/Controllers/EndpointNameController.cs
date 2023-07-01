using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite.Controllers;

public class EndpointNameController : ControllerBase
{
    private readonly LinkGenerator _generator;

    public EndpointNameController(LinkGenerator generator)
    {
        _generator = generator;
    }

    // This is a special case that leads to multiple endpoints with the same route name. IRouter-based routing
    // supports this.
    [HttpGet]
    [HttpPost]
    [Route("/[controller]/[action]/{path?}", Name = "EndpointNameController_LinkToAttributeRouted")]
    public string LinkToAttributeRouted()
    {
        return _generator.GetPathByName(
            HttpContext,
            "EndpointNameController_LinkToAttributeRouted",
            values: null
        );
    }

    public string LinkToConventionalRouted()
    {
        return _generator.GetPathByName(
            HttpContext,
            "RouteWithOptionalSegment",
            new { controller = "EndpointName", action = nameof(LinkToConventionalRouted), }
        );
    }
}
