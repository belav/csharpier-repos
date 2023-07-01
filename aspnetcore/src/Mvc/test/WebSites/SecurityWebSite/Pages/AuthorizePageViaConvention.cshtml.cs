using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecurityWebSite;

public class AuthorizePageViaConvention : PageModel
{
    public IActionResult OnGet() => Page();
}
