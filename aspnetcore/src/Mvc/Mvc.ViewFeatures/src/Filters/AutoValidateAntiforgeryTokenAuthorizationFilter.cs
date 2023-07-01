using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Filters;

internal sealed class AutoValidateAntiforgeryTokenAuthorizationFilter
    : ValidateAntiforgeryTokenAuthorizationFilter
{
    public AutoValidateAntiforgeryTokenAuthorizationFilter(
        IAntiforgery antiforgery,
        ILoggerFactory loggerFactory
    )
        : base(antiforgery, loggerFactory) { }

    protected override bool ShouldValidate(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var method = context.HttpContext.Request.Method;
        if (
            HttpMethods.IsGet(method)
            || HttpMethods.IsHead(method)
            || HttpMethods.IsTrace(method)
            || HttpMethods.IsOptions(method)
        )
        {
            return false;
        }

        // Anything else requires a token.
        return true;
    }
}
