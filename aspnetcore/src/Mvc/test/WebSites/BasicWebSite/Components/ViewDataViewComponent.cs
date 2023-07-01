using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Components;

public class ViewDataViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
