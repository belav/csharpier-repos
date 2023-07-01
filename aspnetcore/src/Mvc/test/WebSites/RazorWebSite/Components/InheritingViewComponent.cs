using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Components;

[ViewComponent(Name = "InheritingViewComponent")]
public class InheritingViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Address address)
    {
        return View("/Views/InheritingInherits/_ViewComponent.cshtml", address);
    }
}
