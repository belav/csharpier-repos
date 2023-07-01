using Microsoft.AspNetCore.Mvc;

namespace CorsWebSite;

[Route("NonCors/[action]")]
public class CustomerController : Controller
{
    [HttpOptions]
    public IEnumerable<string> GetOptions()
    {
        return new[] { "Create", "Update", "Delete" };
    }

    [HttpPost]
    public IEnumerable<string> Post()
    {
        return new[] { "customer1", "customer2", "customer3" };
    }
}
