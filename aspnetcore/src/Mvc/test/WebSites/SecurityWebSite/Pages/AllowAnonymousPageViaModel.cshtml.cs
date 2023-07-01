using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecurityWebSite;

[AllowAnonymous]
public class AllowAnonymousPageViaModel : PageModel
{
    public IActionResult OnGet() => Page();
}
