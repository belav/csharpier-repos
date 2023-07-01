using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite.Pages.Filters;

[TestPageModelFilter]
public class FiltersAppliedToPageAndPageModel : PageModel
{
    public IActionResult OnGet() => Page();
}
