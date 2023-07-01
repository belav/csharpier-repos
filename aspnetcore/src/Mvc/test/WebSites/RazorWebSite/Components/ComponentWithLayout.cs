using Microsoft.AspNetCore.Mvc;

namespace MvcSample.Web.Components;

public class ComponentWithLayout : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        ViewData["Title"] = "ViewComponent With Title";
        return View();
    }
}
