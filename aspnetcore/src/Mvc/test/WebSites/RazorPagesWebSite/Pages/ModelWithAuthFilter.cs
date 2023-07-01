using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

[Authorize]
public class ModelWithAuthFilter : PageModel
{
    public IActionResult OnGet() => Page();
}
