using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace RazorPagesWebSite;

[PageModel]
public class HelloWorldWithPageModelAttributeModel
{
    public string Message { get; set; }

    public void OnGet(string message)
    {
        Message = message;
    }
}
