using Microsoft.AspNetCore.Mvc;

namespace FormatterWebSite.Controllers;

public class SerializableErrorController : Controller
{
    [HttpPost]
    public IActionResult CreateEmployee([FromBody] Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Content("Hello World!");
    }
}
