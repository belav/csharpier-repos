using Microsoft.AspNetCore.Mvc;

namespace SecurityWebSite.Controllers;

[IgnoreAntiforgeryToken]
public class IgnoreAntiforgeryController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index()
    {
        return Content("Ok");
    }
}
