using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

[BindProperties(SupportsGet = true)]
public class BindPropertiesWithSupportsGetOnModel : PageModel
{
    public string Property { get; set; }
}
