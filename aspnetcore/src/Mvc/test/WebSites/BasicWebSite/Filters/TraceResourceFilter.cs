using Microsoft.AspNetCore.Mvc.Filters;

namespace BasicWebSite;

public class TraceResourceFilter : IResourceFilter
{
    public void OnResourceExecuted(ResourceExecutedContext context) { }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.HttpContext.Items[nameof(TraceResourceFilter)] =
            $"This value was set by {nameof(TraceResourceFilter)}";
    }
}
