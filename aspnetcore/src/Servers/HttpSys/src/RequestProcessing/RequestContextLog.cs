// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal static class RequestContextLog
{
    [LoggerMessage(
        LoggerEventIds.RequestError,
        LogLevel.Error,
        "ProcessRequestAsync",
        EventName = "RequestError"
    )]
    partial public static void RequestError(ILogger logger, Exception exception);

    [LoggerMessage(
        LoggerEventIds.RequestProcessError,
        LogLevel.Error,
        "ProcessRequestAsync",
        EventName = "RequestProcessError"
    )]
    partial public static void RequestProcessError(ILogger logger, Exception exception);

    [LoggerMessage(
        LoggerEventIds.RequestsDrained,
        LogLevel.Information,
        "All requests drained.",
        EventName = "RequestsDrained"
    )]
    partial public static void RequestsDrained(ILogger logger);
}
