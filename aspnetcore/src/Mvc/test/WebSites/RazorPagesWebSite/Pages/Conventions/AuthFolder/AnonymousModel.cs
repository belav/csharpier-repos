using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

[AllowAnonymous]
public class AnonymousModel : PageModel
{
    public void OnGet() { }
}
