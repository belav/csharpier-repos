// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Rewrite.Logging;

partial internal static class RewriteMiddlewareLoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "Request is continuing in applying rules. Current url is {currentUrl}",
        EventName = "RequestContinueResults"
    )]
    partial public static void RewriteMiddlewareRequestContinueResults(
        this ILogger logger,
        string currentUrl
    );

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Request is done processing. Location header '{Location}' with status code '{StatusCode}'.",
        EventName = "RequestResponseComplete"
    )]
    partial public static void RewriteMiddlewareRequestResponseComplete(
        this ILogger logger,
        string location,
        int statusCode
    );

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "Request is done applying rules. Url was rewritten to {rewrittenUrl}",
        EventName = "RequestStopRules"
    )]
    partial public static void RewriteMiddlewareRequestStopRules(
        this ILogger logger,
        string rewrittenUrl
    );

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "Request did not match current rule '{Name}'.",
        EventName = "UrlRewriteNotMatchedRule"
    )]
    partial public static void UrlRewriteNotMatchedRule(this ILogger logger, string? name);

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "Request matched current UrlRewriteRule '{Name}'.",
        EventName = "UrlRewriteMatchedRule"
    )]
    partial public static void UrlRewriteMatchedRule(this ILogger logger, string? name);

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "Request matched current ModRewriteRule.",
        EventName = "ModRewriteNotMatchedRule"
    )]
    partial public static void ModRewriteNotMatchedRule(this ILogger logger);

    [LoggerMessage(
        7,
        LogLevel.Debug,
        "Request matched current ModRewriteRule.",
        EventName = "ModRewriteMatchedRule"
    )]
    partial public static void ModRewriteMatchedRule(this ILogger logger);

    [LoggerMessage(
        8,
        LogLevel.Information,
        "Request redirected to HTTPS",
        EventName = "RedirectedToHttps"
    )]
    partial public static void RedirectedToHttps(this ILogger logger);

    [LoggerMessage(
        13,
        LogLevel.Information,
        "Request redirected to www",
        EventName = "RedirectedToWww"
    )]
    partial public static void RedirectedToWww(this ILogger logger);

    [LoggerMessage(
        14,
        LogLevel.Information,
        "Request redirected to root domain from www subdomain",
        EventName = "RedirectedToNonWww"
    )]
    partial public static void RedirectedToNonWww(this ILogger logger);

    [LoggerMessage(
        9,
        LogLevel.Information,
        "Request was redirected to {redirectedUrl}",
        EventName = "RedirectedRequest"
    )]
    partial public static void RedirectedRequest(this ILogger logger, string redirectedUrl);

    [LoggerMessage(
        10,
        LogLevel.Information,
        "Request was rewritten to {rewrittenUrl}",
        EventName = "RewritetenRequest"
    )]
    partial public static void RewrittenRequest(this ILogger logger, string rewrittenUrl);

    [LoggerMessage(
        11,
        LogLevel.Debug,
        "Request to {requestedUrl} was aborted",
        EventName = "AbortedRequest"
    )]
    partial public static void AbortedRequest(this ILogger logger, string requestedUrl);

    [LoggerMessage(
        12,
        LogLevel.Debug,
        "Request to {requestedUrl} was ended",
        EventName = "CustomResponse"
    )]
    partial public static void CustomResponse(this ILogger logger, string requestedUrl);
}
