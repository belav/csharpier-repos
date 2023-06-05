// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "Received a sign-in message without a WResult.",
        EventName = "SignInWithoutWResult"
    )]
    partial public static void SignInWithoutWResult(this ILogger logger);

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Received a sign-in message without a token.",
        EventName = "SignInWithoutToken"
    )]
    partial public static void SignInWithoutToken(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Error,
        "Exception occurred while processing message.",
        EventName = "ExceptionProcessingMessage"
    )]
    partial public static void ExceptionProcessingMessage(this ILogger logger, Exception ex);

    [LoggerMessage(
        4,
        LogLevel.Warning,
        "The sign-out redirect URI '{uri}' is malformed.",
        EventName = "MalformedRedirectUri"
    )]
    partial public static void MalformedRedirectUri(this ILogger logger, string uri);

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "RemoteSignOutContext.HandledResponse",
        EventName = "RemoteSignOutHandledResponse"
    )]
    partial public static void RemoteSignOutHandledResponse(this ILogger logger);

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "RemoteSignOutContext.Skipped",
        EventName = "RemoteSignOutSkipped"
    )]
    partial public static void RemoteSignOutSkipped(this ILogger logger);

    [LoggerMessage(
        7,
        LogLevel.Information,
        "Remote signout request processed.",
        EventName = "RemoteSignOut"
    )]
    partial public static void RemoteSignOut(this ILogger logger);
}
