using Microsoft.AspNetCore.Mvc;

namespace RazorPagesWebSite.Components;

public class ViewDataViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
