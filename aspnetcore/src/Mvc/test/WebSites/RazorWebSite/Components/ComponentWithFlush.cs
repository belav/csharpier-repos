using Microsoft.AspNetCore.Mvc;

namespace MvcSample.Web.Components;

[ViewComponent(Name = "ComponentWithFlush")]
public class ComponentWithFlush : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
