// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Diagnostics;

partial internal static class DiagnosticsLoggerExtensions
{
    // ExceptionHandlerMiddleware & DeveloperExceptionPageMiddleware
    [LoggerMessage(
        1,
        LogLevel.Error,
        "An unhandled exception has occurred while executing the request.",
        EventName = "UnhandledException"
    )]
    partial public static void UnhandledException(this ILogger logger, Exception exception);

    // ExceptionHandlerMiddleware
    [LoggerMessage(
        2,
        LogLevel.Warning,
        "The response has already started, the error handler will not be executed.",
        EventName = "ResponseStarted"
    )]
    partial public static void ResponseStartedErrorHandler(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Error,
        "An exception was thrown attempting to execute the error handler.",
        EventName = "Exception"
    )]
    partial public static void ErrorHandlerException(this ILogger logger, Exception exception);
}

partial internal static class DeveloperExceptionPageMiddlewareLoggerExtensions
{
    [LoggerMessage(
        2,
        LogLevel.Warning,
        "The response has already started, the error page middleware will not be executed.",
        EventName = "ResponseStarted"
    )]
    partial public static void ResponseStartedErrorPageMiddleware(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Error,
        "An exception was thrown attempting to display the error page.",
        EventName = "DisplayErrorPageException"
    )]
    partial public static void DisplayErrorPageException(this ILogger logger, Exception exception);
}
