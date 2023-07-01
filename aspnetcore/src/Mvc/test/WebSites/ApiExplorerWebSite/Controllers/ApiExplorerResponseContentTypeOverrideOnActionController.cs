using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[Produces("text/xml")]
[Route("ApiExplorerResponseContentTypeOverrideOnAction")]
public class ApiExplorerResponseContentTypeOverrideOnActionController : Controller
{
    [HttpGet("Controller")]
    public Product GetController()
    {
        return null;
    }

    [HttpGet("Action")]
    [Produces("application/json")]
    public Product GetAction()
    {
        return null;
    }
}
