using HtmlGenerationWebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace HtmlGenerationWebSite.Controllers;

public class HtmlGeneration_WeirdExpressionsController : Controller
{
    public IActionResult GetWeirdWithHtmlHelpers()
    {
        return View(new WeirdModel());
    }

    public IActionResult GetWeirdWithTagHelpers()
    {
        return View(new WeirdModel());
    }
}
