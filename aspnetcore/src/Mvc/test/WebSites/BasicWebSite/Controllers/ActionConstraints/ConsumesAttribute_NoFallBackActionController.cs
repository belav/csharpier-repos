using BasicWebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Controllers.ActionConstraints;

[Route("ConsumesAttribute_AmbiguousActions/[action]")]
public class ConsumesAttribute_NoFallBackActionController : Controller
{
    [Consumes("application/json", "text/json")]
    public Product CreateProduct([FromBody] Product_Json jsonInput)
    {
        return jsonInput;
    }

    [Consumes("application/xml")]
    public Product CreateProduct([FromBody] Product_Xml xmlInput)
    {
        return xmlInput;
    }
}
