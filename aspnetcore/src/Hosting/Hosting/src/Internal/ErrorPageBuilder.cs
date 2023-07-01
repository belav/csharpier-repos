using System.Reflection;
using Microsoft.AspNetCore.Hosting.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting;

internal static class ErrorPageBuilder
{
    public static RequestDelegate BuildErrorPageApplication(
        IFileProvider contentRootFileProvider,
        ILogger logger,
        bool showDetailedErrors,
        Exception exception
    )
    {
        if (exception is TargetInvocationException tae)
        {
            exception = tae.InnerException!;
        }

        var model = ErrorPageModelBuilder.CreateErrorPageModel(
            contentRootFileProvider,
            logger,
            showDetailedErrors,
            exception
        );

        var errorPage = new ErrorPage(model);
        return context =>
        {
            context.Response.StatusCode = 500;
            context.Response.Headers.CacheControl = "no-cache,no-store";
            context.Response.Headers.Pragma = "no-cache";
            context.Response.ContentType = "text/html; charset=utf-8";
            return errorPage.ExecuteAsync(context);
        };
    }
}
