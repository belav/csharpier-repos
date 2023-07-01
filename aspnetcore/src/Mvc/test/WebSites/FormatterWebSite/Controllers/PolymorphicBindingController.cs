using FormatterWebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormatterWebSite.Controllers;

public class PolymorphicBindingController : ControllerBase
{
    public IActionResult ModelBound([ModelBinder(typeof(PolymorphicBinder))] BaseModel person)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(person);
    }

    [HttpPost]
    public IActionResult InputFormatted([FromBody] IModel person)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(person);
    }
}
