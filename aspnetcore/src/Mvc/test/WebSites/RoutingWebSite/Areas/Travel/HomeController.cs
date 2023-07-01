using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite.Travel;

[Area("Travel")]
public class HomeController : Controller
{
    private readonly TestResponseGenerator _generator;

    public HomeController(TestResponseGenerator generator)
    {
        _generator = generator;
    }

    public IActionResult Index()
    {
        return _generator.Generate("/Travel", "/Travel/Home", "/Travel/Home/Index");
    }

    [HttpGet("ContosoCorp/AboutTravel")]
    public IActionResult About()
    {
        return _generator.Generate();
    }
}
