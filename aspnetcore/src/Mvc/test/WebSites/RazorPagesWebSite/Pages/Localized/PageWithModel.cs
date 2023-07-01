using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite.Pages.Localized;

public class PageWithModel : PageModel
{
    public IActionResult OnGet() => Page();
}
