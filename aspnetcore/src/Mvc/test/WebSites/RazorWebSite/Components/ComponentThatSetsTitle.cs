using Microsoft.AspNetCore.Mvc;

namespace MvcSample.Web.Components;

[ViewComponent(Name = "ComponentThatSetsTitle")]
public class ComponentThatSetsTitle : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
