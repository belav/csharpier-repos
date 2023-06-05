// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal class DisconnectListener
{
    partial private static class Log
    {
        [LoggerMessage(
            LoggerEventIds.DisconnectHandlerError,
            LogLevel.Error,
            "CreateDisconnectToken Callback",
            EventName = "DisconnectHandlerError"
        )]
        partial public static void DisconnectHandlerError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.DisconnectRegistrationError,
            LogLevel.Error,
            "Unable to register for disconnect notifications.",
            EventName = "DisconnectRegistrationError"
        )]
        partial public static void DisconnectRegistrationError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.CreateDisconnectTokenError,
            LogLevel.Error,
            "CreateDisconnectToken",
            EventName = "CreateDisconnectTokenError"
        )]
        partial public static void CreateDisconnectTokenError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.DisconnectTriggered,
            LogLevel.Debug,
            "CreateDisconnectToken; http.sys disconnect callback fired for connection ID: {ConnectionId}",
            EventName = "DisconnectTriggered"
        )]
        partial public static void DisconnectTriggered(ILogger logger, ulong connectionId);

        [LoggerMessage(
            LoggerEventIds.RegisterDisconnectListener,
            LogLevel.Debug,
            "CreateDisconnectToken; Registering connection for disconnect for connection ID: {ConnectionId}",
            EventName = "RegisterDisconnectListener"
        )]
        partial public static void RegisterDisconnectListener(ILogger logger, ulong connectionId);

        [LoggerMessage(
            LoggerEventIds.UnknownDisconnectError,
            LogLevel.Debug,
            "HttpWaitForDisconnectEx",
            EventName = "UnknownDisconnectError"
        )]
        partial public static void UnknownDisconnectError(ILogger logger, Exception exception);
    }
}
