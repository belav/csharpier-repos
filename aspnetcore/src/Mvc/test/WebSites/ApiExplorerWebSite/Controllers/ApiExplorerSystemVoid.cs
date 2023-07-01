using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[Route("ApiExplorerVoid/[action]")]
[ApiController]
public class ApiExplorerVoidController : Controller
{
    [ProducesResponseType(typeof(void), 401)]
    public IActionResult ActionWithVoidType() => Ok();

    [ProducesResponseType(401)]
    public IActionResult ActionWithNoExplicitType() => Ok();
}
