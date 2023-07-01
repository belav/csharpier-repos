using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Controllers;

public class NestedViewStartsController : Controller
{
    public IActionResult Index()
    {
        return View("NestedViewStarts/Index");
    }
}
