// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal class ResponseStreamAsyncResult
{
    partial private static class Log
    {
        [LoggerMessage(
            LoggerEventIds.WriteCancelled,
            LogLevel.Debug,
            "FlushAsync.IOCompleted; Write cancelled with error code: {ErrorCode}",
            EventName = "WriteCancelled"
        )]
        partial public static void WriteCancelled(ILogger logger, uint errorCode);

        [LoggerMessage(
            LoggerEventIds.WriteError,
            LogLevel.Error,
            "FlushAsync.IOCompleted",
            EventName = "WriteError"
        )]
        partial public static void WriteError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.WriteErrorIgnored,
            LogLevel.Debug,
            "FlushAsync.IOCompleted; Ignored write exception: {ErrorCode}",
            EventName = "WriteErrorIgnored"
        )]
        partial public static void WriteErrorIgnored(ILogger logger, uint errorCode);
    }
}
