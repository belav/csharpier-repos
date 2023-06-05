// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.StackTrace.Sources;

partial internal static class LoggerExtensions
{
    [LoggerMessage(
        0,
        LogLevel.Debug,
        "Failed to read stack trace information for exception.",
        EventName = "FailedToReadStackTraceInfo"
    )]
    partial public static void FailedToReadStackTraceInfo(this ILogger logger, Exception exception);
}
