using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[Route("ApiExplorerVisibilityDisabledByConvention")]
public class ApiExplorerVisibilityDisabledByConventionController : Controller
{
    [HttpGet]
    public void Get() { }
}
