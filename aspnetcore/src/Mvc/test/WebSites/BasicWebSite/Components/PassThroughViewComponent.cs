using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Components;

public class PassThroughViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(long value)
    {
        return View(value);
    }
}
