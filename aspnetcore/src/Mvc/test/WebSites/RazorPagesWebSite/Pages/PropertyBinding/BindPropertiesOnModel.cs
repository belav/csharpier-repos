using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

[BindProperties]
public class BindPropertiesOnModel : PageModel
{
    [FromQuery]
    public string Property1 { get; set; }

    public string Property2 { get; set; }
}
