using Microsoft.AspNetCore.Mvc;

namespace SecurityWebSite.Controllers;

[AutoValidateAntiforgeryToken]
public class AntiforgeryController : Controller
{
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult Index()
    {
        return Content("Ok");
    }
}
