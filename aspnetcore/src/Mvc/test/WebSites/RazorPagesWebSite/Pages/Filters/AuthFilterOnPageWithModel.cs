using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite.Pages.Filters;

[SkipStatusCodePages]
public class AuthFilterOnPageWithModel : PageModel
{
    public IActionResult OnGet() => Page();
}
