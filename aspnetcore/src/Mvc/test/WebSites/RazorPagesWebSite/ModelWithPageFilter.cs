using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

[HandlerChangingPageFilter]
public class ModelWithPageFilter : PageModel
{
    public string Message { get; private set; }

    public void OnGet()
    {
        Message = $"Hello from {nameof(OnGet)}";
    }

    public void OnGetEdit()
    {
        Message = $"Hello from {nameof(OnGetEdit)}";
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class HandlerChangingPageFilterAttribute : Attribute, IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
        context.HandlerMethod = context.ActionDescriptor.HandlerMethods.First(
            m => m.Name == "Edit"
        );
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context) { }

    public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
}
