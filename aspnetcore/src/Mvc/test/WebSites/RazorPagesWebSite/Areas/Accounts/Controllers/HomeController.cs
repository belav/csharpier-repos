using Microsoft.AspNetCore.Mvc;

namespace RazorPagesWebSite;

[Area("Accounts")]
public class HomeController : Controller
{
    public IActionResult Index() => View();
}
