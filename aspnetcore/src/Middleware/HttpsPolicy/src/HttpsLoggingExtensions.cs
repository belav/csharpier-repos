// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.HttpsPolicy;

partial internal static class HttpsLoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "Redirecting to '{redirect}'.",
        EventName = "RedirectingToHttps"
    )]
    partial public static void RedirectingToHttps(this ILogger logger, string redirect);

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Https port '{port}' loaded from configuration.",
        EventName = "PortLoadedFromConfig"
    )]
    partial public static void PortLoadedFromConfig(this ILogger logger, int port);

    [LoggerMessage(
        3,
        LogLevel.Warning,
        "Failed to determine the https port for redirect.",
        EventName = "FailedToDeterminePort"
    )]
    partial public static void FailedToDeterminePort(this ILogger logger);

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "Https port '{httpsPort}' discovered from server endpoints.",
        EventName = "PortFromServer"
    )]
    partial public static void PortFromServer(this ILogger logger, int httpsPort);
}
