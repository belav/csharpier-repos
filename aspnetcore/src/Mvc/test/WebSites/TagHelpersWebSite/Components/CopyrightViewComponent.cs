using Microsoft.AspNetCore.Mvc;

namespace TagHelpersWebSite;

public class CopyrightViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string website, int year)
    {
        var dict = new Dictionary<string, object> { ["website"] = website, ["year"] = year };

        return View(dict);
    }
}
