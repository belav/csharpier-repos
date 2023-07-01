using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("ApiExplorerVisibilitySetExplicitly")]
public class ApiExplorerVisibilitySetExplicitlyController : Controller
{
    [ApiExplorerSettings(IgnoreApi = false)]
    [HttpGet("Enabled")]
    public void Enabled() { }

    [HttpGet("Disabled")]
    public void Disabled() { }
}
