using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesClassLibrary;

public class NestedControllerOwner
{
    public class NestedController : Controller
    {
        [HttpGet("/not-discovered/nested")]
        public IActionResult Index()
        {
            return new EmptyResult();
        }
    }
}
