// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.ConcurrencyLimiter;

/// <summary>
/// Limits the number of concurrent requests allowed in the application.
/// </summary>
[Obsolete(
    "Concurrency Limiter middleware has been deprecated and will be removed in a future release. Update the app to use concurrency features in rate limiting middleware. For more information, see https://aka.ms/aspnet/rate-limiting"
)]
public partial class ConcurrencyLimiterMiddleware
{
    private readonly IQueuePolicy _queuePolicy;
    private readonly RequestDelegate _next;
    private readonly RequestDelegate _onRejected;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new <see cref="ConcurrencyLimiterMiddleware"/>.
    /// </summary>
    /// <param name="next">The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used for logging.</param>
    /// <param name="queue">The queueing strategy to use for the server.</param>
    /// <param name="options">The options for the middleware, currently containing the 'OnRejected' callback.</param>
    public ConcurrencyLimiterMiddleware(
        RequestDelegate next,
        ILoggerFactory loggerFactory,
        IQueuePolicy queue,
        IOptions<ConcurrencyLimiterOptions> options
    )
    {
        if (options.Value.OnRejected == null)
        {
            throw new ArgumentException(
                "The value of 'options.OnRejected' must not be null.",
                nameof(options)
            );
        }

        _next = next;
        _logger = loggerFactory.CreateLogger<ConcurrencyLimiterMiddleware>();
        _onRejected = options.Value.OnRejected;
        _queuePolicy = queue;
    }

    /// <summary>
    /// Invokes the logic of the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>A <see cref="Task"/> that completes when the request leaves.</returns>
    public async Task Invoke(HttpContext context)
    {
        var waitInQueueTask = _queuePolicy.TryEnterAsync();

        // Make sure we only ever call GetResult once on the TryEnterAsync ValueTask b/c it resets.
        bool result;

        if (waitInQueueTask.IsCompleted)
        {
            ConcurrencyLimiterEventSource.Log.QueueSkipped();
            result = waitInQueueTask.Result;
        }
        else
        {
            using (ConcurrencyLimiterEventSource.Log.QueueTimer())
            {
                result = await waitInQueueTask;
            }
        }

        if (result)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                _queuePolicy.OnExit();
            }
        }
        else
        {
            ConcurrencyLimiterEventSource.Log.RequestRejected();
            ConcurrencyLimiterLog.RequestRejectedQueueFull(_logger);
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await _onRejected(context);
        }
    }

    private static partial class ConcurrencyLimiterLog
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            "MaxConcurrentRequests limit reached, request has been queued. Current active requests: {ActiveRequests}.",
            EventName = "RequestEnqueued"
        )]
        internal static partial void RequestEnqueued(ILogger logger, int activeRequests);

        [LoggerMessage(
            2,
            LogLevel.Debug,
            "Request dequeued. Current active requests: {ActiveRequests}.",
            EventName = "RequestDequeued"
        )]
        internal static partial void RequestDequeued(ILogger logger, int activeRequests);

        [LoggerMessage(
            3,
            LogLevel.Debug,
            "Below MaxConcurrentRequests limit, running request immediately. Current active requests: {ActiveRequests}",
            EventName = "RequestRunImmediately"
        )]
        internal static partial void RequestRunImmediately(ILogger logger, int activeRequests);

        [LoggerMessage(
            4,
            LogLevel.Debug,
            "Currently at the 'RequestQueueLimit', rejecting this request with a '503 server not available' error",
            EventName = "RequestRejectedQueueFull"
        )]
        internal static partial void RequestRejectedQueueFull(ILogger logger);
    }
}
