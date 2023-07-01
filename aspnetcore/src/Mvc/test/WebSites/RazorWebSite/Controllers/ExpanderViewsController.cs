using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Controllers;

public class ExpanderViewsController : Controller
{
    // This result discovers the Index.cshtml from /View but the partial is executed from /Shared-Views
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Partial()
    {
        return PartialView("_ExpanderPartial");
    }
}
