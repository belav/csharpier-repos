using Microsoft.AspNetCore.Mvc;

namespace FormatterWebSite.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class JsonInputFormatterController
{
    [HttpPost]
    public ActionResult<int> IntValue(int value)
    {
        return value;
    }
}
