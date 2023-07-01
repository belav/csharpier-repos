using Microsoft.AspNetCore.Mvc;

namespace TagHelpersWebSite.Components;

public class GenericViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Dictionary<string, List<string>> items)
    {
        return View(items);
    }
}
