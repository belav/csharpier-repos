using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Controllers;

public class UsersController : Controller
{
    public IActionResult Index()
    {
        return Content("Users.Index");
    }
}
