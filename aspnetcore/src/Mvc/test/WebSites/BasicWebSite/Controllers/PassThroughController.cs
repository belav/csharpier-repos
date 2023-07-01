using BasicWebSite.Components;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Controllers;

public class PassThroughController : Controller
{
    public IActionResult Index(long value)
    {
        return ViewComponent(typeof(PassThroughViewComponent), new { value });
    }
}
