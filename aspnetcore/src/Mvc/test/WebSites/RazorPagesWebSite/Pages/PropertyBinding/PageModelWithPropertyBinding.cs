using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

public class PageModelWithPropertyBinding : PageModel
{
    [ModelBinder]
    public UserModel UserModel { get; set; }

    [FromRoute]
    public int Id { get; set; }

    [BindProperty(SupportsGet = true)]
    [FromQuery]
    public string PropertyWithSupportGetsTrue { get; set; }

    public void OnGet() { }
}
