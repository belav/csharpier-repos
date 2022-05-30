// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.RazorViews;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.StackTrace.Sources;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Diagnostics;

/// <summary>
/// Captures synchronous and asynchronous exceptions from the pipeline and generates error responses.
/// </summary>
public class DeveloperExceptionPageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DeveloperExceptionPageOptions _options;
    private readonly ILogger _logger;
    private readonly IFileProvider _fileProvider;
    private readonly DiagnosticSource _diagnosticSource;
    private readonly ExceptionDetailsProvider _exceptionDetailsProvider;
    private readonly Func<ErrorContext, Task> _exceptionHandler;
    private static readonly MediaTypeHeaderValue _textHtmlMediaType = new MediaTypeHeaderValue("text/html");

    /// <summary>
    /// Initializes a new instance of the <see cref="DeveloperExceptionPageMiddleware"/> class
    /// </summary>
    /// <param name="next"></param>
    /// <param name="options"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="hostingEnvironment"></param>
    /// <param name="diagnosticSource"></param>
    /// <param name="filters"></param>
    public DeveloperExceptionPageMiddleware(
        RequestDelegate next,
        IOptions<DeveloperExceptionPageOptions> options,
        ILoggerFactory loggerFactory,
        IWebHostEnvironment hostingEnvironment,
        DiagnosticSource diagnosticSource,
        IEnumerable<IDeveloperPageExceptionFilter> filters)
    {
        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (filters == null)
        {
            throw new ArgumentNullException(nameof(filters));
        }

        _next = next;
        _options = options.Value;
        _logger = loggerFactory.CreateLogger<DeveloperExceptionPageMiddleware>();
        _fileProvider = _options.FileProvider ?? hostingEnvironment.ContentRootFileProvider;
        _diagnosticSource = diagnosticSource;
        _exceptionDetailsProvider = new ExceptionDetailsProvider(_fileProvider, _logger, _options.SourceCodeLineCount);
        _exceptionHandler = DisplayException;

        foreach (var filter in filters.Reverse())
        {
            var nextFilter = _exceptionHandler;
            _exceptionHandler = errorContext => filter.HandleExceptionAsync(errorContext, nextFilter);
        }
    }

    /// <summary>
    /// Process an individual request.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.UnhandledException(ex);

            if (context.Response.HasStarted)
            {
                _logger.ResponseStartedErrorPageMiddleware();
                throw;
            }

            try
            {
                context.Response.Clear();

                // Preserve the status code that would have been written by the server automatically when a BadHttpRequestException is thrown.
                if (ex is BadHttpRequestException badHttpRequestException)
                {
                    context.Response.StatusCode = badHttpRequestException.StatusCode;
                }
                else
                {
                    context.Response.StatusCode = 500;
                }

                await _exceptionHandler(new ErrorContext(context, ex));

                const string eventName = "Microsoft.AspNetCore.Diagnostics.UnhandledException";
                if (_diagnosticSource.IsEnabled(eventName))
                {
                    WriteDiagnosticEvent(_diagnosticSource, eventName, new { httpContext = context, exception = ex });
                }

                return;
            }
            catch (Exception ex2)
            {
                // If there's a Exception while generating the error page, re-throw the original exception.
                _logger.DisplayErrorPageException(ex2);
            }
            throw;
        }

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026",
            Justification = "The values being passed into Write have the commonly used properties being preserved with DynamicDependency.")]
        static void WriteDiagnosticEvent<TValue>(DiagnosticSource diagnosticSource, string name, TValue value)
            => diagnosticSource.Write(name, value);
    }

    // Assumes the response headers have not been sent.  If they have, still attempt to write to the body.
    private Task DisplayException(ErrorContext errorContext)
    {
        var httpContext = errorContext.HttpContext;
        var headers = httpContext.Request.GetTypedHeaders();
        var acceptHeader = headers.Accept;

        // If the client does not ask for HTML just format the exception as plain text
        if (acceptHeader == null || !acceptHeader.Any(h => h.IsSubsetOf(_textHtmlMediaType)))
        {
            httpContext.Response.ContentType = "text/plain; charset=utf-8";

            var sb = new StringBuilder();
            sb.AppendLine(errorContext.Exception.ToString());
            sb.AppendLine();
            sb.AppendLine("HEADERS");
            sb.AppendLine("=======");
            foreach (var pair in httpContext.Request.Headers)
            {
                sb.AppendLine(FormattableString.Invariant($"{pair.Key}: {pair.Value}"));
            }

            return httpContext.Response.WriteAsync(sb.ToString());
        }

        if (errorContext.Exception is ICompilationException compilationException)
        {
            return DisplayCompilationException(httpContext, compilationException);
        }

        return DisplayRuntimeException(httpContext, errorContext.Exception);
    }

    private Task DisplayCompilationException(
        HttpContext context,
        ICompilationException compilationException)
    {
        var model = new CompilationErrorPageModel(_options);

        var errorPage = new CompilationErrorPage(model);

        if (compilationException.CompilationFailures == null)
        {
            return errorPage.ExecuteAsync(context);
        }

        foreach (var compilationFailure in compilationException.CompilationFailures)
        {
            if (compilationFailure == null)
            {
                continue;
            }

            var stackFrames = new List<StackFrameSourceCodeInfo>();
            var exceptionDetails = new ExceptionDetails(compilationFailure.FailureSummary!, stackFrames);
            model.ErrorDetails.Add(exceptionDetails);
            model.CompiledContent.Add(compilationFailure.CompiledContent);

            if (compilationFailure.Messages == null)
            {
                continue;
            }

            var sourceLines = compilationFailure
                    .SourceFileContent?
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var item in compilationFailure.Messages)
            {
                if (item == null)
                {
                    continue;
                }

                var frame = new StackFrameSourceCodeInfo
                {
                    File = compilationFailure.SourceFilePath,
                    Line = item.StartLine,
                    Function = string.Empty
                };

                if (sourceLines != null)
                {
                    _exceptionDetailsProvider.ReadFrameContent(frame, sourceLines, item.StartLine, item.EndLine);
                }

                frame.ErrorDetails = item.Message;

                stackFrames.Add(frame);
            }
        }

        return errorPage.ExecuteAsync(context);
    }

    private Task DisplayRuntimeException(HttpContext context, Exception ex)
    {
        var endpoint = context.GetEndpoint();

        EndpointModel? endpointModel = null;
        if (endpoint != null)
        {
            endpointModel = new EndpointModel();
            endpointModel.DisplayName = endpoint.DisplayName;

            if (endpoint is RouteEndpoint routeEndpoint)
            {
                endpointModel.RoutePattern = routeEndpoint.RoutePattern.RawText;
                endpointModel.Order = routeEndpoint.Order;

                var httpMethods = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods;
                if (httpMethods != null)
                {
                    endpointModel.HttpMethods = string.Join(", ", httpMethods);
                }
            }
        }

        var request = context.Request;
        var title = Resources.ErrorPageHtml_Title;

        if (ex is BadHttpRequestException badHttpRequestException)
        {
            var badRequestReasonPhrase = WebUtilities.ReasonPhrases.GetReasonPhrase(badHttpRequestException.StatusCode);

            if (!string.IsNullOrEmpty(badRequestReasonPhrase))
            {
                title = badRequestReasonPhrase;
            }
        }

        var model = new ErrorPageModel
        {
            Options = _options,
            ErrorDetails = _exceptionDetailsProvider.GetDetails(ex),
            Query = request.Query,
            Cookies = request.Cookies,
            Headers = request.Headers,
            RouteValues = request.RouteValues,
            Endpoint = endpointModel,
            Title = title,
        };

        var errorPage = new ErrorPage(model);
        return errorPage.ExecuteAsync(context);
    }
}
