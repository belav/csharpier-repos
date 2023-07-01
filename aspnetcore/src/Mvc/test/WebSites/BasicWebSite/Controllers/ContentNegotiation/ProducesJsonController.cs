using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Controllers.ContentNegotiation;

public class ProducesJsonController : Controller
{
    [Produces("application/xml")]
    public IActionResult Produces_WithNonObjectResult()
    {
        return new JsonResult(new { MethodName = "Produces_WithNonObjectResult" });
    }
}
