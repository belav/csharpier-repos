using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Controllers;

public class BackSlashController : Controller
{
    public IActionResult Index() => View(@"Views\BackSlash\BackSlashView.cshtml");
}
