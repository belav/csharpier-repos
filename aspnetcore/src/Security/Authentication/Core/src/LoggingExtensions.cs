// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(
        4,
        LogLevel.Information,
        "Error from RemoteAuthentication: {ErrorMessage}.",
        EventName = "RemoteAuthenticationFailed"
    )]
    partial public static void RemoteAuthenticationError(this ILogger logger, string errorMessage);

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "The SigningIn event returned Handled.",
        EventName = "SignInHandled"
    )]
    partial public static void SignInHandled(this ILogger logger);

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "The SigningIn event returned Skipped.",
        EventName = "SignInSkipped"
    )]
    partial public static void SignInSkipped(this ILogger logger);

    [LoggerMessage(
        7,
        LogLevel.Information,
        "{AuthenticationScheme} was not authenticated. Failure message: {FailureMessage}",
        EventName = "AuthenticationSchemeNotAuthenticatedWithFailure"
    )]
    partial public static void AuthenticationSchemeNotAuthenticatedWithFailure(
        this ILogger logger,
        string authenticationScheme,
        string failureMessage
    );

    [LoggerMessage(
        8,
        LogLevel.Debug,
        "AuthenticationScheme: {AuthenticationScheme} was successfully authenticated.",
        EventName = "AuthenticationSchemeAuthenticated"
    )]
    partial public static void AuthenticationSchemeAuthenticated(
        this ILogger logger,
        string authenticationScheme
    );

    [LoggerMessage(
        9,
        LogLevel.Debug,
        "AuthenticationScheme: {AuthenticationScheme} was not authenticated.",
        EventName = "AuthenticationSchemeNotAuthenticated"
    )]
    partial public static void AuthenticationSchemeNotAuthenticated(
        this ILogger logger,
        string authenticationScheme
    );

    [LoggerMessage(
        12,
        LogLevel.Information,
        "AuthenticationScheme: {AuthenticationScheme} was challenged.",
        EventName = "AuthenticationSchemeChallenged"
    )]
    partial public static void AuthenticationSchemeChallenged(
        this ILogger logger,
        string authenticationScheme
    );

    [LoggerMessage(
        13,
        LogLevel.Information,
        "AuthenticationScheme: {AuthenticationScheme} was forbidden.",
        EventName = "AuthenticationSchemeForbidden"
    )]
    partial public static void AuthenticationSchemeForbidden(
        this ILogger logger,
        string authenticationScheme
    );

    [LoggerMessage(
        14,
        LogLevel.Warning,
        "{CorrelationProperty} state property not found.",
        EventName = "CorrelationPropertyNotFound"
    )]
    partial public static void CorrelationPropertyNotFound(
        this ILogger logger,
        string correlationProperty
    );

    [LoggerMessage(
        15,
        LogLevel.Warning,
        "'{CorrelationCookieName}' cookie not found.",
        EventName = "CorrelationCookieNotFound"
    )]
    partial public static void CorrelationCookieNotFound(
        this ILogger logger,
        string correlationCookieName
    );

    [LoggerMessage(
        16,
        LogLevel.Warning,
        "The correlation cookie value '{CorrelationCookieName}' did not match the expected value '{CorrelationCookieValue}'.",
        EventName = "UnexpectedCorrelationCookieValue"
    )]
    partial public static void UnexpectedCorrelationCookieValue(
        this ILogger logger,
        string correlationCookieName,
        string correlationCookieValue
    );

    [LoggerMessage(
        17,
        LogLevel.Information,
        "Access was denied by the resource owner or by the remote server.",
        EventName = "AccessDenied"
    )]
    partial public static void AccessDeniedError(this ILogger logger);

    [LoggerMessage(
        18,
        LogLevel.Debug,
        "The AccessDenied event returned Handled.",
        EventName = "AccessDeniedContextHandled"
    )]
    partial public static void AccessDeniedContextHandled(this ILogger logger);

    [LoggerMessage(
        19,
        LogLevel.Debug,
        "The AccessDenied event returned Skipped.",
        EventName = "AccessDeniedContextSkipped"
    )]
    partial public static void AccessDeniedContextSkipped(this ILogger logger);
}
