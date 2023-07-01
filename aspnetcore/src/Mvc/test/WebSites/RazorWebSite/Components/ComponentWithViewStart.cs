using Microsoft.AspNetCore.Mvc;

namespace MvcSample.Web.Components;

public class ComponentWithViewStart : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        ViewData["Title"] = "ViewComponent With ViewStart";
        return View();
    }
}
