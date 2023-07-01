using FormatterWebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormatterWebSite.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestApiController : ControllerBase
{
    [HttpPost]
    public IActionResult PostBookWithNoValidation(BookModelWithNoValidation bookModel) => Ok();
}
