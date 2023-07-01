using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite;

public class ConventionalTransformerController : Controller
{
    private readonly TestResponseGenerator _generator;

    public ConventionalTransformerController(TestResponseGenerator generator)
    {
        _generator = generator;
    }

    public IActionResult Index()
    {
        return _generator.Generate();
    }

    public IActionResult Param(string param)
    {
        return _generator.Generate(
            $"/ConventionalTransformerRoute/conventional-transformer/Param/{param}"
        );
    }
}
