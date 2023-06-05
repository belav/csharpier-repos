// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.SignalR;

partial internal static class HubConnectionHandlerLog
{
    [LoggerMessage(
        1,
        LogLevel.Error,
        "Error when dispatching '{HubMethod}' on hub.",
        EventName = "ErrorDispatchingHubEvent"
    )]
    partial public static void ErrorDispatchingHubEvent(
        ILogger logger,
        string hubMethod,
        Exception exception
    );

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Error when processing requests.",
        EventName = "ErrorProcessingRequest"
    )]
    partial public static void ErrorProcessingRequest(ILogger logger, Exception exception);

    [LoggerMessage(3, LogLevel.Trace, "Abort callback failed.", EventName = "AbortFailed")]
    partial public static void AbortFailed(ILogger logger, Exception exception);

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "Error when sending Close message.",
        EventName = "ErrorSendingClose"
    )]
    partial public static void ErrorSendingClose(ILogger logger, Exception exception);

    [LoggerMessage(5, LogLevel.Debug, "OnConnectedAsync started.", EventName = "ConnectedStarting")]
    partial public static void ConnectedStarting(ILogger logger);

    [LoggerMessage(6, LogLevel.Debug, "OnConnectedAsync ending.", EventName = "ConnectedEnding")]
    partial public static void ConnectedEnding(ILogger logger);
}
