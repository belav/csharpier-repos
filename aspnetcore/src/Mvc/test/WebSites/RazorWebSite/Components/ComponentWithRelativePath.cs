using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Components;

public class ComponentWithRelativePath : ViewComponent
{
    public IViewComponentResult Invoke(Person person)
    {
        return View("../Shared/Components/ComponentWithRelativePath.cshtml", person);
    }
}
