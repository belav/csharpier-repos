using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesClassLibrary;

public class Inventory : ResourcesController
{
    [HttpGet]
    public IActionResult Get()
    {
        return new ContentResult { Content = "4" };
    }
}
