using Microsoft.AspNetCore.Mvc;

namespace RazorPagesWebSite;

public class ClientValidationDisabledController : Controller
{
    [HttpGet("/Controller/ClientValidationDisabled")]
    public IActionResult ValidationDisabled() => View();
}
