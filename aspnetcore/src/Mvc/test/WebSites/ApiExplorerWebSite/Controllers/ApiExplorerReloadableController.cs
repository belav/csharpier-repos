using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[Route("ApiExplorerReload")]
public class ApiExplorerReloadableController : Controller
{
    [Route("Index")]
    [Reload]
    public string Index() => "Hello world";

    [Route("Reload")]
    [PassThru]
    public IActionResult Reload([FromServices] WellKnownChangeToken changeToken)
    {
        changeToken.TokenSource.Cancel();
        return Ok();
    }
}
