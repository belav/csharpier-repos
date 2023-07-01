using Microsoft.AspNetCore.Mvc;

namespace FormatterWebSite.Controllers;

public class RespectBrowserAcceptHeaderController : Controller
{
    [HttpGet]
    public string ReturnString()
    {
        return "Hello World!";
    }
}
