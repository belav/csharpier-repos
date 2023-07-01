using Microsoft.AspNetCore.Mvc;

namespace Mvc.RoutingWebSite.Controllers;

[Route("ConsumesAttribute/[action]")]
public class ConsumesAttributeController : Controller
{
    [HttpPost]
    [Consumes("application/json")]
    public IActionResult Json([FromBody] string json)
    {
        return Content($"Received json \"{json}\"");
    }
}
