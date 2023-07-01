using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite.Areas.Order;

[Area("Order")]
[Route("Order/[action]", Name = "[area]_[action]")]
public class OrderController : Controller
{
    private readonly TestResponseGenerator _generator;

    public OrderController(TestResponseGenerator generator)
    {
        _generator = generator;
    }

    [HttpGet]
    public IActionResult GetOrder()
    {
        return _generator.Generate("/Order/GetOrder");
    }
}
