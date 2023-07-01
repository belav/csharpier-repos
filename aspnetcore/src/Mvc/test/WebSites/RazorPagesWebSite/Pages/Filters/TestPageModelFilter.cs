using Microsoft.AspNetCore.Mvc.Filters;

namespace RazorPagesWebSite.Pages.Filters;

public class TestPageModelFilter : Attribute, IResourceFilter
{
    public void OnResourceExecuted(ResourceExecutedContext context) { }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.HttpContext.Response.Headers["PageModelFilterKey"] = "PageModelFilterValue";
    }
}
