using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Controllers;

public class TemplateExpander : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ViewWithLayout()
    {
        return View();
    }
}
