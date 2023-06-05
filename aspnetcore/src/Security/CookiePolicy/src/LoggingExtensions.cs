// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(1, LogLevel.Trace, "Needs consent: {needsConsent}.", EventName = "NeedsConsent")]
    partial public static void NeedsConsent(this ILogger logger, bool needsConsent);

    [LoggerMessage(2, LogLevel.Trace, "Has consent: {hasConsent}.", EventName = "HasConsent")]
    partial public static void HasConsent(this ILogger logger, bool hasConsent);

    [LoggerMessage(3, LogLevel.Debug, "Consent granted.", EventName = "ConsentGranted")]
    partial public static void ConsentGranted(this ILogger logger);

    [LoggerMessage(4, LogLevel.Debug, "Consent withdrawn.", EventName = "ConsentWithdrawn")]
    partial public static void ConsentWithdrawn(this ILogger logger);

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "Cookie '{key}' suppressed due to consent policy.",
        EventName = "CookieSuppressed"
    )]
    partial public static void CookieSuppressed(this ILogger logger, string key);

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "Delete cookie '{key}' suppressed due to developer policy.",
        EventName = "DeleteCookieSuppressed"
    )]
    partial public static void DeleteCookieSuppressed(this ILogger logger, string key);

    [LoggerMessage(
        7,
        LogLevel.Debug,
        "Cookie '{key}' upgraded to 'secure'.",
        EventName = "UpgradedToSecure"
    )]
    partial public static void CookieUpgradedToSecure(this ILogger logger, string key);

    [LoggerMessage(
        8,
        LogLevel.Debug,
        "Cookie '{key}' same site mode upgraded to '{mode}'.",
        EventName = "UpgradedSameSite"
    )]
    partial public static void CookieSameSiteUpgraded(this ILogger logger, string key, string mode);

    [LoggerMessage(
        9,
        LogLevel.Debug,
        "Cookie '{key}' upgraded to 'httponly'.",
        EventName = "UpgradedToHttpOnly"
    )]
    partial public static void CookieUpgradedToHttpOnly(this ILogger logger, string key);
}
