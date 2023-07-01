using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesWebSite;

public class NotInServicesController : Controller
{
    [HttpGet("/not-discovered/not-in-services")]
    public IActionResult Index()
    {
        return View();
    }
}
