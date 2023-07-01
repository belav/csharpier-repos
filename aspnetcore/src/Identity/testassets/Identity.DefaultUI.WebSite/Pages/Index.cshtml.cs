using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.DefaultUI.WebSite.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    public void OnGet() { }
}
