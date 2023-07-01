using Microsoft.AspNetCore.Mvc;

namespace MvcSandbox.Controllers;

[Route("[controller]/[action]")]
public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
