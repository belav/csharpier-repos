using BasicWebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Areas.Area1.Controllers;

[Area("Area1")]
[Route("[area]/[controller]/[action]")]
public class RemoteAttribute_HomeController : Controller
{
    private static RemoteAttributeUser _user;

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(RemoteAttributeUser user)
    {
        ModelState.Remove("id");
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        _user = user;
        return RedirectToAction(nameof(Details));
    }

    public IActionResult Details()
    {
        return View(_user);
    }
}
