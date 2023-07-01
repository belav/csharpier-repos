using Microsoft.AspNetCore.Mvc;

namespace HtmlGenerationWebSite.Areas.Customer.Controllers;

[Area("Customer")]
public class HtmlGeneration_CustomerController : Controller
{
    public IActionResult Index(Models.Customer customer)
    {
        return View("Customer");
    }

    [HttpGet]
    public IActionResult CustomerWithRecords()
    {
        return View("CustomerWithRecords");
    }

    public IActionResult CustomerWithRecords(Models.CustomerRecord customer)
    {
        return View("CustomerWithRecords");
    }
}
