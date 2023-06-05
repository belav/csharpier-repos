// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(
        10,
        LogLevel.Information,
        "AuthenticationScheme: {AuthenticationScheme} signed in.",
        EventName = "AuthenticationSchemeSignedIn"
    )]
    partial public static void AuthenticationSchemeSignedIn(
        this ILogger logger,
        string authenticationScheme
    );

    [LoggerMessage(
        11,
        LogLevel.Information,
        "AuthenticationScheme: {AuthenticationScheme} signed out.",
        EventName = "AuthenticationSchemeSignedOut"
    )]
    partial public static void AuthenticationSchemeSignedOut(
        this ILogger logger,
        string authenticationScheme
    );
}
