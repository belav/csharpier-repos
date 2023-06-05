// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal class HttpSysListener
{
    partial private static class Log
    {
        [LoggerMessage(
            LoggerEventIds.ListenerDisposeError,
            LogLevel.Error,
            "Dispose",
            EventName = "ListenerDisposeError"
        )]
        partial public static void ListenerDisposeError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ListenerDisposing,
            LogLevel.Trace,
            "Disposing the listener.",
            EventName = "ListenerDisposing"
        )]
        partial public static void ListenerDisposing(ILogger logger);

        [LoggerMessage(
            LoggerEventIds.HttpSysListenerCtorError,
            LogLevel.Error,
            ".Ctor",
            EventName = "HttpSysListenerCtorError"
        )]
        partial public static void HttpSysListenerCtorError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ListenerStartError,
            LogLevel.Error,
            "Start",
            EventName = "ListenerStartError"
        )]
        partial public static void ListenerStartError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ListenerStarting,
            LogLevel.Trace,
            "Starting the listener.",
            EventName = "ListenerStarting"
        )]
        partial public static void ListenerStarting(ILogger logger);

        [LoggerMessage(
            LoggerEventIds.ListenerStopError,
            LogLevel.Error,
            "Stop",
            EventName = "ListenerStopError"
        )]
        partial public static void ListenerStopError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ListenerStopping,
            LogLevel.Trace,
            "Stopping the listener.",
            EventName = "ListenerStopping"
        )]
        partial public static void ListenerStopping(ILogger logger);

        [LoggerMessage(
            LoggerEventIds.RequestValidationFailed,
            LogLevel.Error,
            "Error validating request {RequestId}",
            EventName = "RequestValidationFailed"
        )]
        partial public static void RequestValidationFailed(
            ILogger logger,
            Exception exception,
            ulong requestId
        );
    }
}
