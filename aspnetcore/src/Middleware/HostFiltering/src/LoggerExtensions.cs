// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.HostFiltering;

partial internal static class LoggerExtensions
{
    [LoggerMessage(
        0,
        LogLevel.Debug,
        "Wildcard detected, all requests with hosts will be allowed.",
        EventName = "WildcardDetected"
    )]
    partial public static void WildcardDetected(this ILogger logger);

    [LoggerMessage(
        1,
        LogLevel.Debug,
        "Allowed hosts: {Hosts}",
        EventName = "AllowedHosts",
        SkipEnabledCheck = true
    )]
    partial public static void AllowedHosts(this ILogger logger, string hosts);

    [LoggerMessage(2, LogLevel.Trace, "All hosts are allowed.", EventName = "AllHostsAllowed")]
    partial public static void AllHostsAllowed(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Information,
        "{Protocol} request rejected due to missing or empty host header.",
        EventName = "RequestRejectedMissingHost"
    )]
    partial public static void RequestRejectedMissingHost(this ILogger logger, string protocol);

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "{Protocol} request allowed with missing or empty host header.",
        EventName = "RequestAllowedMissingHost"
    )]
    partial public static void RequestAllowedMissingHost(this ILogger logger, string protocol);

    [LoggerMessage(
        5,
        LogLevel.Trace,
        "The host '{Host}' matches an allowed host.",
        EventName = "AllowedHostMatched"
    )]
    partial public static void AllowedHostMatched(this ILogger logger, string host);

    [LoggerMessage(
        6,
        LogLevel.Information,
        "The host '{Host}' does not match an allowed host.",
        EventName = "NoAllowedHostMatched"
    )]
    partial public static void NoAllowedHostMatched(this ILogger logger, string host);
}
