using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RoutingWebSite.Pages;

public class LGAnotherPageModel : PageModel
{
    private readonly LinkGenerator _linkGenerator;

    public LGAnotherPageModel(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public ContentResult OnGet()
    {
        return Content(
            _linkGenerator.GetPathByAction(
                HttpContext,
                action: nameof(LG2Controller.SomeAction),
                controller: "LG2"
            )
        );
    }
}
