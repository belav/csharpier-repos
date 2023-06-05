// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Filters;

partial internal class ValidateAntiforgeryTokenAuthorizationFilter
    : IAsyncAuthorizationFilter,
        IAntiforgeryPolicy
{
    private readonly IAntiforgery _antiforgery;
    private readonly ILogger _logger;

    public ValidateAntiforgeryTokenAuthorizationFilter(
        IAntiforgery antiforgery,
        ILoggerFactory loggerFactory
    )
    {
        if (antiforgery == null)
        {
            throw new ArgumentNullException(nameof(antiforgery));
        }

        _antiforgery = antiforgery;
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
        {
            Log.NotMostEffectiveFilter(_logger, typeof(IAntiforgeryPolicy));
            return;
        }

        if (ShouldValidate(context))
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException exception)
            {
                Log.AntiforgeryTokenInvalid(_logger, exception.Message, exception);
                context.Result = new AntiforgeryValidationFailedResult();
            }
        }
    }

    protected virtual bool ShouldValidate(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return true;
    }

    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Information,
            "Antiforgery token validation failed. {Message}",
            EventName = "AntiforgeryTokenInvalid"
        )]
        partial public static void AntiforgeryTokenInvalid(
            ILogger logger,
            string message,
            Exception exception
        );

        [LoggerMessage(
            2,
            LogLevel.Trace,
            "Skipping the execution of current filter as its not the most effective filter implementing the policy {FilterPolicy}.",
            EventName = "NotMostEffectiveFilter"
        )]
        partial public static void NotMostEffectiveFilter(ILogger logger, Type filterPolicy);
    }
}
