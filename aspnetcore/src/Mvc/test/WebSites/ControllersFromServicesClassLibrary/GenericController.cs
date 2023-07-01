using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesClassLibrary;

public class GenericController<TController> : Controller
{
    [HttpGet("/not-discovered/generic")]
    public IActionResult Index()
    {
        return new EmptyResult();
    }
}
