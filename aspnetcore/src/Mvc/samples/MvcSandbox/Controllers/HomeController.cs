using Microsoft.AspNetCore.Mvc;

namespace MvcSandbox.Controllers;

public class HomeController : Controller
{
    [ModelBinder]
    public string Id { get; set; }

    public IActionResult Index()
    {
        return View();
    }
}
