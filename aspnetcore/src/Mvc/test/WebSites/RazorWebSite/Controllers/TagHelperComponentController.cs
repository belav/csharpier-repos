using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite;

public class TagHelperComponentController : Controller
{
    // GET: /<controller>/
    public IActionResult GetHead()
    {
        return View("Head");
    }

    public IActionResult GetBody()
    {
        return View("Body");
    }
}
