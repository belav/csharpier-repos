using Microsoft.AspNetCore.Mvc.Filters;

namespace RazorPagesWebSite.Pages.Filters;

public class TestPageFilter : Attribute, IPageFilter
{
    public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        context.HttpContext.Response.Headers["PageFilterKey"] = "PageFilterValue";
    }

    public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
}
