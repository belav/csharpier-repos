using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

[ResponseCache(Duration = 10, Location = ResponseCacheLocation.Client)]
public class ModelWithResponseCache : PageModel
{
    public string Message { get; set; }

    public void OnGet()
    {
        Message = $"Hello from {nameof(ModelWithResponseCache)}.{nameof(OnGet)}";
    }
}
