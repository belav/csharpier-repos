using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BasicWebSite;

public class TraceResultOutputFilter : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var trace = context.HttpContext.Items[nameof(TraceResourceFilter)];
        context.Result = new ObjectResult(trace);
    }
}
