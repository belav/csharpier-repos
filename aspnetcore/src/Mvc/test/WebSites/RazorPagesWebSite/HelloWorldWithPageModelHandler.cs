using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

public class HelloWorldWithPageModelHandler : PageModel
{
    public string Message { get; set; }

    public void OnGet(string message)
    {
        Message = message;
    }

    public void OnPost()
    {
        Message = "You posted!";
    }
}
