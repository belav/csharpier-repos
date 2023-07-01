using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecurityWebSite;

[Authorize("RequireClaimB")]
public class AuthorizePageViaModel : PageModel
{
    public IActionResult OnGet() => Page();
}
