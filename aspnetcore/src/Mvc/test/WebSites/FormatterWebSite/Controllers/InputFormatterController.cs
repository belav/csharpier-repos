using Microsoft.AspNetCore.Mvc;

namespace FormatterWebSite.Controllers;

public class InputFormatterController : Controller
{
    public IActionResult ReturnInput([FromBody] string test)
    {
        if (!ModelState.IsValid)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        return Content(test);
    }
}
