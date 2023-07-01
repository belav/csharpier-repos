using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[Route("ApiExplorerNameSetByConvention")]
public class ApiExplorerNameSetByConventionController : Controller
{
    [HttpGet]
    public void Get() { }
}
