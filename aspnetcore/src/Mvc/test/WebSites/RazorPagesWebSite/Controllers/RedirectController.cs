using Microsoft.AspNetCore.Mvc;

namespace RazorPagesWebSite;

public class RedirectController : Controller
{
    [HttpGet("/RedirectToPage")]
    public IActionResult RedirectToPageAction()
    {
        return RedirectToPage("/RedirectToController", new { param = 17 });
    }
}
