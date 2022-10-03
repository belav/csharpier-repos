// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for enabling <see cref="ExceptionHandlerExtensions"/>.
/// </summary>
public static class ExceptionHandlerExtensions
{
    /// <summary>
    /// Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
    /// The request will not be re-executed if the response has already started.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return SetExceptionHandlerMiddleware(app, options: null);
    }

    /// <summary>
    /// Adds a middleware to the pipeline that will catch exceptions, log them, reset the request path, and re-execute the request.
    /// The request will not be re-executed if the response has already started.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="errorHandlingPath"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, string errorHandlingPath)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandlingPath = new PathString(errorHandlingPath)
        });
    }

    /// <summary>
    /// Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
    /// The request will not be re-executed if the response has already started.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, Action<IApplicationBuilder> configure)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var subAppBuilder = app.New();
        configure(subAppBuilder);
        var exceptionHandlerPipeline = subAppBuilder.Build();

        return app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = exceptionHandlerPipeline
        });
    }

    /// <summary>
    /// Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
    /// The request will not be re-executed if the response has already started.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, ExceptionHandlerOptions options)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var iOptions = Options.Create(options);
        return SetExceptionHandlerMiddleware(app, iOptions);
    }

    private static IApplicationBuilder SetExceptionHandlerMiddleware(IApplicationBuilder app, IOptions<ExceptionHandlerOptions>? options)
    {
        var problemDetailsService = app.ApplicationServices.GetService<IProblemDetailsService>();

        app.Properties["analysis.NextMiddlewareName"] = "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware";

        // Only use this path if there's a global router (in the 'WebApplication' case).
        if (app.Properties.TryGetValue(RerouteHelper.GlobalRouteBuilderKey, out var routeBuilder) && routeBuilder is not null)
        {
            return app.Use(next =>
            {
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                var diagnosticListener = app.ApplicationServices.GetRequiredService<DiagnosticListener>();

                if (options is null)
                {
                    options = app.ApplicationServices.GetRequiredService<IOptions<ExceptionHandlerOptions>>();
                }

                if (!string.IsNullOrEmpty(options.Value.ExceptionHandlingPath) && options.Value.ExceptionHandler is null)
                {
                    var newNext = RerouteHelper.Reroute(app, routeBuilder, next);
                    // store the pipeline for the error case
                    options.Value.ExceptionHandler = newNext;
                }

                return new ExceptionHandlerMiddlewareImpl(next, loggerFactory, options, diagnosticListener, problemDetailsService).Invoke;
            });
        }

        if (options is null)
        {
            return app.UseMiddleware<ExceptionHandlerMiddlewareImpl>();
        }

        return app.UseMiddleware<ExceptionHandlerMiddlewareImpl>(options);
    }
}
