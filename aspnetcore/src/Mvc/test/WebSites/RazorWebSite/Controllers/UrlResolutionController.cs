using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Controllers;

public class UrlResolutionController : Controller
{
    public IActionResult Index()
    {
        var model = new Person { Name = "John Doe" };

        return View(model);
    }
}
