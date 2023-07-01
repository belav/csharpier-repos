using Microsoft.AspNetCore.Mvc;

namespace ApplicationModelWebSite.Controllers;

[MultipleAreas("Products", "Services", "Manage")]
public class MultipleAreasController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
