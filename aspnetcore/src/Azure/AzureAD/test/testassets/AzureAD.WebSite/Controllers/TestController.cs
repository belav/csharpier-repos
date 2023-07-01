using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureAD.WebSite.Controllers;

public class TestController : Controller
{
    [Authorize]
    [HttpGet("/api/get")]
    public IActionResult Get() => Ok();
}
