using Microsoft.AspNetCore.Mvc;

namespace MvcSample.Web.Components;

public class SplashViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var region = (string)ViewData["Locale"];
        var model = region == "North" ? "NorthWest Store" : "Nationwide Store";

        return View(model: model);
    }
}
