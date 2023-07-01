using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RazorPagesWebSite.Controllers;

[Route("[controller]/[action]")]
[Authorize]
public class AuthorizedActionController : Controller
{
    public IActionResult Index() => Ok();
}
