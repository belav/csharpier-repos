using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[Route("ApiExplorerVisibilityEnabledByConvention")]
public class ApiExplorerVisibilityEnabledByConventionController : Controller
{
    [HttpGet]
    public void Get() { }
}
