using Microsoft.AspNetCore.Mvc;

namespace MvcSample.Web.Components;

[ViewComponent(Name = "ComponentForViewWithPaths")]
public class ComponentForViewWithPaths : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
