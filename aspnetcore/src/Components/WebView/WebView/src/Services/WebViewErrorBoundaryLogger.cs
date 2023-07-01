using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Components.WebView.Services;

internal sealed class WebViewErrorBoundaryLogger : IErrorBoundaryLogger
{
    private readonly ILogger<ErrorBoundary> _errorBoundaryLogger;

    public WebViewErrorBoundaryLogger(ILogger<ErrorBoundary> errorBoundaryLogger)
    {
        _errorBoundaryLogger = errorBoundaryLogger;
    }

    public ValueTask LogErrorAsync(Exception exception)
    {
        // For, client-side code, all internal state is visible to the end user. We can just
        // log directly to the console.
        _errorBoundaryLogger.LogError(exception, exception.ToString());
        return ValueTask.CompletedTask;
    }
}
