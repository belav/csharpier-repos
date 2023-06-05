// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.SignalR;

partial public class HubConnectionContext
{
    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            "Completed connection handshake. Using HubProtocol '{Protocol}'.",
            EventName = "HandshakeComplete"
        )]
        partial public static void HandshakeComplete(ILogger logger, string protocol);

        [LoggerMessage(
            2,
            LogLevel.Debug,
            "Handshake was canceled.",
            EventName = "HandshakeCanceled"
        )]
        partial public static void HandshakeCanceled(ILogger logger);

        [LoggerMessage(
            3,
            LogLevel.Trace,
            "Sent a ping message to the client.",
            EventName = "SentPing"
        )]
        partial public static void SentPing(ILogger logger);

        [LoggerMessage(
            4,
            LogLevel.Debug,
            "Unable to send Ping message to client, the transport buffer is full.",
            EventName = "TransportBufferFull"
        )]
        partial public static void TransportBufferFull(ILogger logger);

        [LoggerMessage(
            5,
            LogLevel.Debug,
            "Failed connection handshake.",
            EventName = "HandshakeFailed"
        )]
        partial public static void HandshakeFailed(ILogger logger, Exception? exception);

        [LoggerMessage(
            6,
            LogLevel.Error,
            "Failed writing message. Aborting connection.",
            EventName = "FailedWritingMessage"
        )]
        partial public static void FailedWritingMessage(ILogger logger, Exception exception);

        [LoggerMessage(
            7,
            LogLevel.Debug,
            "Server does not support version {Version} of the {Protocol} protocol.",
            EventName = "ProtocolVersionFailed"
        )]
        partial public static void ProtocolVersionFailed(
            ILogger logger,
            string protocol,
            int version
        );

        [LoggerMessage(8, LogLevel.Trace, "Abort callback failed.", EventName = "AbortFailed")]
        partial public static void AbortFailed(ILogger logger, Exception exception);

        [LoggerMessage(
            9,
            LogLevel.Debug,
            "Client timeout ({ClientTimeout}ms) elapsed without receiving a message from the client. Closing connection.",
            EventName = "ClientTimeout"
        )]
        partial public static void ClientTimeout(ILogger logger, TimeSpan clientTimeout);

        [LoggerMessage(
            10,
            LogLevel.Debug,
            "The maximum message size of {MaxMessageSize}B was exceeded while parsing the Handshake. The message size can be configured in AddHubOptions.",
            EventName = "HandshakeSizeLimitExceeded"
        )]
        partial public static void HandshakeSizeLimitExceeded(ILogger logger, long maxMessageSize);
    }
}
