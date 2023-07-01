using HtmlGenerationWebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace HtmlGenerationWebSite.Controllers;

public class CheckViewData : Controller
{
    public IActionResult AtViewModel()
    {
        return View(new SuperViewModel());
    }

    public IActionResult NullViewModel()
    {
        return View("AtViewModel");
    }

    public IActionResult ViewModel()
    {
        return View(new SuperViewModel());
    }
}
