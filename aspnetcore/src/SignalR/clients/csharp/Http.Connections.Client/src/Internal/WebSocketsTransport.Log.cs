// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Http.Connections.Client.Internal;

partial internal sealed class WebSocketsTransport
{
    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Information,
            "Starting transport. Transfer mode: {TransferFormat}. Url: '{WebSocketUrl}'.",
            EventName = "StartTransport"
        )]
        partial public static void StartTransport(
            ILogger logger,
            TransferFormat transferFormat,
            Uri webSocketUrl
        );

        [LoggerMessage(2, LogLevel.Debug, "Transport stopped.", EventName = "TransportStopped")]
        partial public static void TransportStopped(ILogger logger, Exception? exception);

        [LoggerMessage(3, LogLevel.Debug, "Starting receive loop.", EventName = "StartReceive")]
        partial public static void StartReceive(ILogger logger);

        [LoggerMessage(
            6,
            LogLevel.Information,
            "Transport is stopping.",
            EventName = "TransportStopping"
        )]
        partial public static void TransportStopping(ILogger logger);

        [LoggerMessage(
            10,
            LogLevel.Debug,
            "Passing message to application. Payload size: {Count}.",
            EventName = "MessageToApp"
        )]
        partial public static void MessageToApp(ILogger logger, int count);

        [LoggerMessage(5, LogLevel.Debug, "Receive loop canceled.", EventName = "ReceiveCanceled")]
        partial public static void ReceiveCanceled(ILogger logger);

        [LoggerMessage(4, LogLevel.Debug, "Receive loop stopped.", EventName = "ReceiveStopped")]
        partial public static void ReceiveStopped(ILogger logger);

        [LoggerMessage(7, LogLevel.Debug, "Starting the send loop.", EventName = "SendStarted")]
        partial public static void SendStarted(ILogger logger);

        [LoggerMessage(9, LogLevel.Debug, "Send loop canceled.", EventName = "SendCanceled")]
        partial public static void SendCanceled(ILogger logger);

        [LoggerMessage(8, LogLevel.Debug, "Send loop stopped.", EventName = "SendStopped")]
        partial public static void SendStopped(ILogger logger);

        [LoggerMessage(
            11,
            LogLevel.Information,
            "WebSocket closed by the server. Close status {CloseStatus}.",
            EventName = "WebSocketClosed"
        )]
        partial public static void WebSocketClosed(
            ILogger logger,
            WebSocketCloseStatus? closeStatus
        );

        [LoggerMessage(
            12,
            LogLevel.Debug,
            "Message received. Type: {MessageType}, size: {Count}, EndOfMessage: {EndOfMessage}.",
            EventName = "MessageReceived"
        )]
        partial public static void MessageReceived(
            ILogger logger,
            WebSocketMessageType messageType,
            int count,
            bool endOfMessage
        );

        [LoggerMessage(
            13,
            LogLevel.Debug,
            "Received message from application. Payload size: {Count}.",
            EventName = "ReceivedFromApp"
        )]
        partial public static void ReceivedFromApp(ILogger logger, long count);

        [LoggerMessage(
            14,
            LogLevel.Information,
            "Sending a message canceled.",
            EventName = "SendMessageCanceled"
        )]
        partial public static void SendMessageCanceled(ILogger logger);

        [LoggerMessage(
            15,
            LogLevel.Error,
            "Error while sending a message.",
            EventName = "ErrorSendingMessage"
        )]
        partial public static void ErrorSendingMessage(ILogger logger, Exception exception);

        [LoggerMessage(
            16,
            LogLevel.Information,
            "Closing WebSocket.",
            EventName = "ClosingWebSocket"
        )]
        partial public static void ClosingWebSocket(ILogger logger);

        [LoggerMessage(
            17,
            LogLevel.Debug,
            "Closing webSocket failed.",
            EventName = "ClosingWebSocketFailed"
        )]
        partial public static void ClosingWebSocketFailed(ILogger logger, Exception exception);

        [LoggerMessage(
            18,
            LogLevel.Debug,
            "Canceled passing message to application.",
            EventName = "CancelMessage"
        )]
        partial public static void CancelMessage(ILogger logger);

        [LoggerMessage(19, LogLevel.Debug, "Started transport.", EventName = "StartedTransport")]
        partial public static void StartedTransport(ILogger logger);

        [LoggerMessage(
            20,
            LogLevel.Warning,
            $"Configuring request headers using {nameof(HttpConnectionOptions)}.{nameof(HttpConnectionOptions.Headers)} is not supported when using websockets transport "
                + "on the browser platform.",
            EventName = "HeadersNotSupported"
        )]
        partial public static void HeadersNotSupported(ILogger logger);
    }
}
