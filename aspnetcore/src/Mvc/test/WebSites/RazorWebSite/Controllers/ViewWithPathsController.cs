using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Controllers;

public class ViewWithPathsController : Controller
{
    [HttpGet("/ViewWithPaths")]
    public IActionResult Index()
    {
        return View();
    }
}
