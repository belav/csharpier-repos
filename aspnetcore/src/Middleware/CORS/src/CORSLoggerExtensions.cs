// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Cors;

partial internal static class CORSLoggerExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "The request is a preflight request.",
        EventName = "IsPreflightRequest"
    )]
    partial public static void IsPreflightRequest(this ILogger logger);

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "The request has an origin header: '{origin}'.",
        EventName = "RequestHasOriginHeader"
    )]
    partial public static void RequestHasOriginHeader(this ILogger logger, string origin);

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "The request does not have an origin header.",
        EventName = "RequestDoesNotHaveOriginHeader"
    )]
    partial public static void RequestDoesNotHaveOriginHeader(this ILogger logger);

    [LoggerMessage(
        4,
        LogLevel.Information,
        "CORS policy execution successful.",
        EventName = "PolicySuccess"
    )]
    partial public static void PolicySuccess(this ILogger logger);

    [LoggerMessage(
        5,
        LogLevel.Information,
        "CORS policy execution failed.",
        EventName = "PolicyFailure"
    )]
    partial public static void PolicyFailure(this ILogger logger);

    [LoggerMessage(
        6,
        LogLevel.Information,
        "Request origin {origin} does not have permission to access the resource.",
        EventName = "OriginNotAllowed"
    )]
    partial public static void OriginNotAllowed(this ILogger logger, string origin);

    [LoggerMessage(
        7,
        LogLevel.Information,
        "Request method {accessControlRequestMethod} not allowed in CORS policy.",
        EventName = "AccessControlMethodNotAllowed"
    )]
    partial public static void AccessControlMethodNotAllowed(
        this ILogger logger,
        string accessControlRequestMethod
    );

    [LoggerMessage(
        8,
        LogLevel.Information,
        "Request header '{requestHeader}' not allowed in CORS policy.",
        EventName = "RequestHeaderNotAllowed"
    )]
    partial public static void RequestHeaderNotAllowed(this ILogger logger, string requestHeader);

    [LoggerMessage(
        9,
        LogLevel.Warning,
        "Failed to apply CORS Response headers.",
        EventName = "FailedToSetCorsHeaders"
    )]
    partial public static void FailedToSetCorsHeaders(this ILogger logger, Exception? exception);

    [LoggerMessage(
        10,
        LogLevel.Information,
        "No CORS policy found for the specified request.",
        EventName = "NoCorsPolicyFound"
    )]
    partial public static void NoCorsPolicyFound(this ILogger logger);

    [LoggerMessage(
        12,
        LogLevel.Debug,
        "This request uses the HTTP OPTIONS method but does not have an Access-Control-Request-Method header. This request will not be treated as a CORS preflight request.",
        EventName = "IsNotPreflightRequest"
    )]
    partial public static void IsNotPreflightRequest(this ILogger logger);
}
