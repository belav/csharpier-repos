// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Http.Connections.Client;

partial public class HttpConnection
{
    partial internal static class Log
    {
        [LoggerMessage(1, LogLevel.Debug, "Starting HttpConnection.", EventName = "Starting")]
        partial public static void Starting(ILogger logger);

        [LoggerMessage(
            2,
            LogLevel.Debug,
            "Skipping start, connection is already started.",
            EventName = "SkippingStart"
        )]
        partial public static void SkippingStart(ILogger logger);

        [LoggerMessage(3, LogLevel.Information, "HttpConnection Started.", EventName = "Started")]
        partial public static void Started(ILogger logger);

        [LoggerMessage(
            4,
            LogLevel.Debug,
            "Disposing HttpConnection.",
            EventName = "DisposingHttpConnection"
        )]
        partial public static void DisposingHttpConnection(ILogger logger);

        [LoggerMessage(
            5,
            LogLevel.Debug,
            "Skipping dispose, connection is already disposed.",
            EventName = "SkippingDispose"
        )]
        partial public static void SkippingDispose(ILogger logger);

        [LoggerMessage(6, LogLevel.Information, "HttpConnection Disposed.", EventName = "Disposed")]
        partial public static void Disposed(ILogger logger);

        public static void StartingTransport(
            ILogger logger,
            HttpTransportType transportType,
            Uri url
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                StartingTransport(logger, transportType.ToString(), url);
            }
        }

        [LoggerMessage(
            7,
            LogLevel.Debug,
            "Starting transport '{Transport}' with Url: {Url}.",
            EventName = "StartingTransport",
            SkipEnabledCheck = true
        )]
        partial private static void StartingTransport(ILogger logger, string transport, Uri url);

        [LoggerMessage(
            8,
            LogLevel.Debug,
            "Establishing connection with server at '{Url}'.",
            EventName = "EstablishingConnection"
        )]
        partial public static void EstablishingConnection(ILogger logger, Uri url);

        [LoggerMessage(
            9,
            LogLevel.Debug,
            "Established connection '{ConnectionId}' with the server.",
            EventName = "Established"
        )]
        partial public static void ConnectionEstablished(ILogger logger, string connectionId);

        [LoggerMessage(
            10,
            LogLevel.Error,
            "Failed to start connection. Error getting negotiation response from '{Url}'.",
            EventName = "ErrorWithNegotiation"
        )]
        partial public static void ErrorWithNegotiation(
            ILogger logger,
            Uri url,
            Exception exception
        );

        [LoggerMessage(
            11,
            LogLevel.Error,
            "Failed to start connection. Error starting transport '{Transport}'.",
            EventName = "ErrorStartingTransport"
        )]
        partial public static void ErrorStartingTransport(
            ILogger logger,
            HttpTransportType transport,
            Exception exception
        );

        [LoggerMessage(
            12,
            LogLevel.Debug,
            "Skipping transport {TransportName} because it is not supported by this client.",
            EventName = "TransportNotSupported"
        )]
        partial public static void TransportNotSupported(ILogger logger, string transportName);

        public static void TransportDoesNotSupportTransferFormat(
            ILogger logger,
            HttpTransportType transport,
            TransferFormat transferFormat
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                TransportDoesNotSupportTransferFormat(
                    logger,
                    transport.ToString(),
                    transferFormat.ToString()
                );
            }
        }

        [LoggerMessage(
            13,
            LogLevel.Debug,
            "Skipping transport {TransportName} because it does not support the requested transfer format '{TransferFormat}'.",
            EventName = "TransportDoesNotSupportTransferFormat",
            SkipEnabledCheck = true
        )]
        partial private static void TransportDoesNotSupportTransferFormat(
            ILogger logger,
            string transportName,
            string transferFormat
        );

        public static void TransportDisabledByClient(ILogger logger, HttpTransportType transport)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                TransportDisabledByClient(logger, transport.ToString());
            }
        }

        [LoggerMessage(
            14,
            LogLevel.Debug,
            "Skipping transport {TransportName} because it was disabled by the client.",
            EventName = "TransportDisabledByClient",
            SkipEnabledCheck = true
        )]
        partial public static void TransportDisabledByClient(ILogger logger, string transportName);

        public static void TransportFailed(
            ILogger logger,
            HttpTransportType transport,
            Exception ex
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                TransportFailed(logger, transport.ToString(), ex);
            }
        }

        [LoggerMessage(
            15,
            LogLevel.Debug,
            "Skipping transport {TransportName} because it failed to initialize.",
            EventName = "TransportFailed",
            SkipEnabledCheck = true
        )]
        partial public static void TransportFailed(
            ILogger logger,
            string transportName,
            Exception ex
        );

        [LoggerMessage(
            16,
            LogLevel.Debug,
            "Skipping WebSockets because they are not supported by the operating system.",
            EventName = "WebSocketsNotSupportedByOperatingSystem"
        )]
        partial public static void WebSocketsNotSupportedByOperatingSystem(ILogger logger);

        [LoggerMessage(
            17,
            LogLevel.Error,
            "The transport threw an exception while stopping.",
            EventName = "TransportThrewExceptionOnStop"
        )]
        partial public static void TransportThrewExceptionOnStop(ILogger logger, Exception ex);

        [LoggerMessage(
            18,
            LogLevel.Debug,
            "Transport '{Transport}' started.",
            EventName = "TransportStarted"
        )]
        partial public static void TransportStarted(ILogger logger, HttpTransportType transport);

        [LoggerMessage(
            19,
            LogLevel.Debug,
            "Skipping ServerSentEvents because they are not supported by the browser.",
            EventName = "ServerSentEventsNotSupportedByBrowser"
        )]
        partial public static void ServerSentEventsNotSupportedByBrowser(ILogger logger);

        [LoggerMessage(
            20,
            LogLevel.Trace,
            "Cookies are not supported on this platform.",
            EventName = "CookiesNotSupported"
        )]
        partial public static void CookiesNotSupported(ILogger logger);

        [LoggerMessage(
            21,
            LogLevel.Debug,
            "{StatusCode} received, getting a new access token and retrying request.",
            EventName = "RetryAccessToken"
        )]
        partial public static void RetryAccessToken(ILogger logger, HttpStatusCode statusCode);
    }
}
