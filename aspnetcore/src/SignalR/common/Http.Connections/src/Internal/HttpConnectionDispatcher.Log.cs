// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Http.Connections.Internal;

partial internal sealed class HttpConnectionDispatcher
{
    partial internal static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            "Connection {TransportConnectionId} was disposed.",
            EventName = "ConnectionDisposed"
        )]
        partial public static void ConnectionDisposed(ILogger logger, string transportConnectionId);

        [LoggerMessage(
            2,
            LogLevel.Debug,
            "Connection {TransportConnectionId} is already active via {RequestId}.",
            EventName = "ConnectionAlreadyActive"
        )]
        partial public static void ConnectionAlreadyActive(
            ILogger logger,
            string transportConnectionId,
            string requestId
        );

        [LoggerMessage(
            3,
            LogLevel.Trace,
            "Previous poll canceled for {TransportConnectionId} on {RequestId}.",
            EventName = "PollCanceled"
        )]
        partial public static void PollCanceled(
            ILogger logger,
            string transportConnectionId,
            string requestId
        );

        [LoggerMessage(
            4,
            LogLevel.Debug,
            "Establishing new connection.",
            EventName = "EstablishedConnection"
        )]
        partial public static void EstablishedConnection(ILogger logger);

        [LoggerMessage(
            5,
            LogLevel.Debug,
            "Resuming existing connection.",
            EventName = "ResumingConnection"
        )]
        partial public static void ResumingConnection(ILogger logger);

        [LoggerMessage(6, LogLevel.Trace, "Received {Count} bytes.", EventName = "ReceivedBytes")]
        partial public static void ReceivedBytes(ILogger logger, long count);

        [LoggerMessage(
            7,
            LogLevel.Debug,
            "{TransportType} transport not supported by this connection handler.",
            EventName = "TransportNotSupported"
        )]
        partial public static void TransportNotSupported(
            ILogger logger,
            HttpTransportType transportType
        );

        [LoggerMessage(
            8,
            LogLevel.Debug,
            "Cannot change transports mid-connection; currently using {TransportType}, requesting {RequestedTransport}.",
            EventName = "CannotChangeTransport"
        )]
        partial public static void CannotChangeTransport(
            ILogger logger,
            HttpTransportType transportType,
            HttpTransportType requestedTransport
        );

        [LoggerMessage(
            9,
            LogLevel.Debug,
            "POST requests are not allowed for websocket connections.",
            EventName = "PostNotAllowedForWebSockets"
        )]
        partial public static void PostNotAllowedForWebSockets(ILogger logger);

        [LoggerMessage(
            10,
            LogLevel.Debug,
            "Sending negotiation response.",
            EventName = "NegotiationRequest"
        )]
        partial public static void NegotiationRequest(ILogger logger);

        [LoggerMessage(
            11,
            LogLevel.Trace,
            "Received DELETE request for unsupported transport: {TransportType}.",
            EventName = "ReceivedDeleteRequestForUnsupportedTransport"
        )]
        partial public static void ReceivedDeleteRequestForUnsupportedTransport(
            ILogger logger,
            HttpTransportType transportType
        );

        [LoggerMessage(
            12,
            LogLevel.Trace,
            "Terminating Long Polling connection due to a DELETE request.",
            EventName = "TerminatingConection"
        )]
        partial public static void TerminatingConnection(ILogger logger);

        [LoggerMessage(
            13,
            LogLevel.Debug,
            "Connection {TransportConnectionId} was disposed while a write was in progress.",
            EventName = "ConnectionDisposedWhileWriteInProgress"
        )]
        partial public static void ConnectionDisposedWhileWriteInProgress(
            ILogger logger,
            string transportConnectionId,
            Exception ex
        );

        [LoggerMessage(
            14,
            LogLevel.Debug,
            "Connection {TransportConnectionId} failed to read the HTTP request body.",
            EventName = "FailedToReadHttpRequestBody"
        )]
        partial public static void FailedToReadHttpRequestBody(
            ILogger logger,
            string transportConnectionId,
            Exception ex
        );

        [LoggerMessage(
            15,
            LogLevel.Debug,
            "The client requested version '{clientProtocolVersion}', but the server does not support this version.",
            EventName = "NegotiateProtocolVersionMismatch"
        )]
        partial public static void NegotiateProtocolVersionMismatch(
            ILogger logger,
            int clientProtocolVersion
        );

        [LoggerMessage(
            16,
            LogLevel.Debug,
            "The client requested an invalid protocol version '{queryStringVersionValue}'",
            EventName = "InvalidNegotiateProtocolVersion"
        )]
        partial public static void InvalidNegotiateProtocolVersion(
            ILogger logger,
            string queryStringVersionValue
        );
    }
}
