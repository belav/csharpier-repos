// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class NegotiateLoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Information,
        "Incomplete Negotiate handshake, sending an additional 401 Negotiate challenge.",
        EventName = "IncompleteNegotiateChallenge"
    )]
    partial public static void IncompleteNegotiateChallenge(this ILogger logger);

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Completed Negotiate authentication.",
        EventName = "NegotiateComplete"
    )]
    partial public static void NegotiateComplete(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "Enabling credential persistence for a complete Kerberos handshake.",
        EventName = "EnablingCredentialPersistence"
    )]
    partial public static void EnablingCredentialPersistence(this ILogger logger);

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "Disabling credential persistence for a complete {protocol} handshake.",
        EventName = "DisablingCredentialPersistence"
    )]
    partial public static void DisablingCredentialPersistence(this ILogger logger, string protocol);

    [LoggerMessage(
        5,
        LogLevel.Error,
        "An exception occurred while processing the authentication request.",
        EventName = "ExceptionProcessingAuth"
    )]
    partial public static void ExceptionProcessingAuth(this ILogger logger, Exception ex);

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "Challenged 401 Negotiate.",
        EventName = "ChallengeNegotiate"
    )]
    partial public static void ChallengeNegotiate(this ILogger logger);

    [LoggerMessage(
        7,
        LogLevel.Debug,
        "Negotiate data received for an already authenticated connection, Re-authenticating.",
        EventName = "Reauthenticating"
    )]
    partial public static void Reauthenticating(this ILogger logger);

    [LoggerMessage(
        8,
        LogLevel.Information,
        "Deferring to the server's implementation of Windows Authentication.",
        EventName = "Deferring"
    )]
    partial public static void Deferring(this ILogger logger);

    [LoggerMessage(
        9,
        LogLevel.Debug,
        "There was a problem with the users credentials.",
        EventName = "CredentialError"
    )]
    partial public static void CredentialError(this ILogger logger, Exception ex);

    [LoggerMessage(
        10,
        LogLevel.Debug,
        "The users authentication request was invalid.",
        EventName = "ClientError"
    )]
    partial public static void ClientError(this ILogger logger, Exception ex);

    [LoggerMessage(
        11,
        LogLevel.Debug,
        "Negotiate error code: {error}.",
        EventName = "NegotiateError"
    )]
    partial public static void NegotiateError(this ILogger logger, string error);

    [LoggerMessage(
        12,
        LogLevel.Debug,
        "Negotiate is not supported with {protocol}.",
        EventName = "ProtocolNotSupported"
    )]
    partial public static void ProtocolNotSupported(this ILogger logger, string protocol);
}
