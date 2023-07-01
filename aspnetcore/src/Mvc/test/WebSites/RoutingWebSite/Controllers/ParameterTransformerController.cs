using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite;

[Route("[controller]/[action]", Name = "[controller]_[action]")]
public class ParameterTransformerController : Controller
{
    private readonly TestResponseGenerator _generator;

    public ParameterTransformerController(TestResponseGenerator generator)
    {
        _generator = generator;
    }

    public IActionResult MyAction()
    {
        return _generator.Generate("/parameter-transformer/my-action");
    }
}
