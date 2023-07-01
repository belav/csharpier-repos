using Microsoft.AspNetCore.Mvc;

namespace RazorPagesWebSite;

public class CustomActionResult : IActionResult
{
    public Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = 200;
        return context.HttpContext.Response.WriteAsync(nameof(CustomActionResult));
    }
}
