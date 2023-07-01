using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWebSite.Pages;

public class LGPageModel : PageModel
{
    private readonly LinkGenerator _linkGenerator;

    public LGPageModel(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public ContentResult OnGet()
    {
        return Content(_linkGenerator.GetPathByPage(HttpContext, "./LGAnotherPage"));
    }
}
