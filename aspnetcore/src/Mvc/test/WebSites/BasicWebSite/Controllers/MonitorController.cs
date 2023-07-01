using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace BasicWebSite;

public class MonitorController : Controller
{
    private readonly ActionDescriptorCreationCounter _counterService;

    public MonitorController(IEnumerable<IActionDescriptorProvider> providers)
    {
        _counterService = providers.OfType<ActionDescriptorCreationCounter>().Single();
    }

    public IActionResult CountActionDescriptorInvocations()
    {
        return Content(_counterService.CallCount.ToString(CultureInfo.InvariantCulture));
    }
}
