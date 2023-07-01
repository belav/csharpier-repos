using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecurityWebSite.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [AutoValidateAntiforgeryToken]
    [Authorize]
    [HttpPost]
    public IActionResult AutoAntiforgery()
    {
        return Content("Automaticaly doesn't matter");
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Antiforgery()
    {
        return Content("Doesn't matter");
    }

    public IActionResult Login()
    {
        return Content("Login!");
    }

    public IActionResult Logout()
    {
        return Content("Logout!");
    }
}
