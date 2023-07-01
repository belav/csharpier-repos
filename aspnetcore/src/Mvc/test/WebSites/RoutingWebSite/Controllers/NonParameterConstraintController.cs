using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite;

public class NonParameterConstraintController : Controller
{
    private readonly TestResponseGenerator _generator;

    public NonParameterConstraintController(TestResponseGenerator generator)
    {
        _generator = generator;
    }

    public IActionResult Index()
    {
        return _generator.Generate("/NonParameterConstraintRoute/NonParameterConstraint/Index");
    }
}
