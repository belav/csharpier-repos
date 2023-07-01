using Microsoft.AspNetCore.Mvc;
using Mvc.RoutingWebSite.Infrastructure;

namespace Mvc.RoutingWebSite.Controllers;

public class ConventionalControllerWithMetadata : Controller
{
    [Metadata("C")]
    public IActionResult GetMetadata()
    {
        return Ok(
            HttpContext
                .GetEndpoint()
                .Metadata.GetOrderedMetadata<MetadataAttribute>()
                .Select(m => m.Value)
        );
    }
}
