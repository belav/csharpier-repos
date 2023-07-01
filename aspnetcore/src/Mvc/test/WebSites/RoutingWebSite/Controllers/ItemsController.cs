using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite;

[Route("Items/[action]")]
public class ItemsController : Controller
{
    public ActionResult<Dictionary<object, object>> Index()
    {
        return Ok(HttpContext.Items);
    }

    public string IndexWithSelectiveFilter()
    {
        return "Default response";
    }

    [Route("{arg}")]
    public ActionResult<Dictionary<object, object>> IndexWithArgument(string arg)
    {
        return Ok(HttpContext.Items);
    }
}
