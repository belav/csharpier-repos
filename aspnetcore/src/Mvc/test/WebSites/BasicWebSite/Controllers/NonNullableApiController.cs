using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Controllers;

[ApiController]
[Route("api/NonNullable")]
public class NonNullableApiController : ControllerBase
{
    // GET: api/<controller>
    [HttpGet]
    public ActionResult<string> Get(string language = "pt-br")
    {
        return language;
    }
}
