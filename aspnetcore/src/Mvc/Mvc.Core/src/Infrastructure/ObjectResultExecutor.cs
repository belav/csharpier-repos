// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Infrastructure;

/// <summary>
/// Executes an <see cref="ObjectResult"/> to write to the response.
/// </summary>
public class ObjectResultExecutor : IActionResultExecutor<ObjectResult>
{
    /// <summary>
    /// Creates a new <see cref="ObjectResultExecutor"/>.
    /// </summary>
    /// <param name="formatterSelector">The <see cref="OutputFormatterSelector"/>.</param>
    /// <param name="writerFactory">The <see cref="IHttpResponseStreamWriterFactory"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    /// <param name="mvcOptions">Accessor to <see cref="MvcOptions"/>.</param>
    public ObjectResultExecutor(
        OutputFormatterSelector formatterSelector,
        IHttpResponseStreamWriterFactory writerFactory,
        ILoggerFactory loggerFactory,
        IOptions<MvcOptions> mvcOptions)
    {
        if (formatterSelector == null)
        {
            throw new ArgumentNullException(nameof(formatterSelector));
        }

        if (writerFactory == null)
        {
            throw new ArgumentNullException(nameof(writerFactory));
        }

        if (loggerFactory == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        FormatterSelector = formatterSelector;
        WriterFactory = writerFactory.CreateWriter;
        Logger = loggerFactory.CreateLogger<ObjectResultExecutor>();
    }

    /// <summary>
    /// Gets the <see cref="ILogger"/>.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="OutputFormatterSelector"/>.
    /// </summary>
    protected OutputFormatterSelector FormatterSelector { get; }

    /// <summary>
    /// Gets the writer factory delegate.
    /// </summary>
    protected Func<Stream, Encoding, TextWriter> WriterFactory { get; }

    /// <summary>
    /// Executes the <see cref="ObjectResult"/>.
    /// </summary>
    /// <param name="context">The <see cref="ActionContext"/> for the current request.</param>
    /// <param name="result">The <see cref="ObjectResult"/>.</param>
    /// <returns>
    /// A <see cref="Task"/> which will complete once the <see cref="ObjectResult"/> is written to the response.
    /// </returns>
    public virtual Task ExecuteAsync(ActionContext context, ObjectResult result)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        InferContentTypes(context, result);

        var objectType = result.DeclaredType;

        if (objectType == null || objectType == typeof(object))
        {
            objectType = result.Value?.GetType();
        }

        var value = result.Value;
        return ExecuteAsyncCore(context, result, objectType, value);
    }

    private Task ExecuteAsyncCore(ActionContext context, ObjectResult result, Type? objectType, object? value)
    {
        var formatterContext = new OutputFormatterWriteContext(
            context.HttpContext,
            WriterFactory,
            objectType,
            value);

        var selectedFormatter = FormatterSelector.SelectFormatter(
            formatterContext,
            (IList<IOutputFormatter>)result.Formatters ?? Array.Empty<IOutputFormatter>(),
            result.ContentTypes);
        if (selectedFormatter == null)
        {
            // No formatter supports this.
            Logger.NoFormatter(formatterContext, result.ContentTypes);

            context.HttpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
            return Task.CompletedTask;
        }

        Logger.ObjectResultExecuting(result, value);

        result.OnFormatting(context);
        return selectedFormatter.WriteAsync(formatterContext);
    }

    private static void InferContentTypes(ActionContext context, ObjectResult result)
    {
        Debug.Assert(result.ContentTypes != null);

        // If the user sets the content type both on the ObjectResult (example: by Produces) and Response object,
        // then the one set on ObjectResult takes precedence over the Response object
        var responseContentType = context.HttpContext.Response.ContentType;
        if (result.ContentTypes.Count == 0 && !string.IsNullOrEmpty(responseContentType))
        {
            result.ContentTypes.Add(responseContentType);
        }

        if (result.Value is ProblemDetails)
        {
            result.ContentTypes.Add("application/problem+json");
            result.ContentTypes.Add("application/problem+xml");
        }
    }

    // Removed Log.
    // new EventId(1, "BufferingAsyncEnumerable")
}
