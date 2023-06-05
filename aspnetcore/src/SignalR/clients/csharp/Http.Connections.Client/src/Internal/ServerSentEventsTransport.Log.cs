// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Http.Connections.Client.Internal;

partial internal sealed class ServerSentEventsTransport
{
    partial
    // EventIds 100 - 106 used in SendUtils

    private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Information,
            "Starting transport. Transfer mode: {TransferFormat}.",
            EventName = "StartTransport"
        )]
        partial public static void StartTransport(ILogger logger, TransferFormat transferFormat);

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
            7,
            LogLevel.Debug,
            "Passing message to application. Payload size: {Count}.",
            EventName = "MessageToApplication"
        )]
        partial public static void MessageToApplication(ILogger logger, int count);

        [LoggerMessage(5, LogLevel.Debug, "Receive loop canceled.", EventName = "ReceiveCanceled")]
        partial public static void ReceiveCanceled(ILogger logger);

        [LoggerMessage(4, LogLevel.Debug, "Receive loop stopped.", EventName = "ReceiveStopped")]
        partial public static void ReceiveStopped(ILogger logger);

        [LoggerMessage(
            8,
            LogLevel.Debug,
            "Server-Sent Event Stream ended.",
            EventName = "EventStreamEnded"
        )]
        partial public static void EventStreamEnded(ILogger logger);

        [LoggerMessage(
            9,
            LogLevel.Debug,
            "Received {Count} bytes. Parsing SSE frame.",
            EventName = "ParsingSSE"
        )]
        partial public static void ParsingSSE(ILogger logger, long count);
    }
}
