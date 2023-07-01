using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecurityWebSite;

public class AllowAnonymousPageViaConvention : PageModel
{
    public IActionResult OnGet() => Page();
}
