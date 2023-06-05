// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Information,
        "Failed to validate the token.",
        EventName = "TokenValidationFailed"
    )]
    partial public static void TokenValidationFailed(this ILogger logger, Exception ex);

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Successfully validated the token.",
        EventName = "TokenValidationSucceeded"
    )]
    partial public static void TokenValidationSucceeded(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Error,
        "Exception occurred while processing message.",
        EventName = "ProcessingMessageFailed"
    )]
    partial public static void ErrorProcessingMessage(this ILogger logger, Exception ex);
}
