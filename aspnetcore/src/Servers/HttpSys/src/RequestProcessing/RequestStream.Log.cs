// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal class RequestStream
{
    partial private static class Log
    {
        [LoggerMessage(
            LoggerEventIds.ErrorWhenReadAsync,
            LogLevel.Debug,
            "ReadAsync",
            EventName = "ErrorWhenReadAsync"
        )]
        partial public static void ErrorWhenReadAsync(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ErrorWhenReadBegun,
            LogLevel.Debug,
            "BeginRead",
            EventName = "ErrorWhenReadBegun"
        )]
        partial public static void ErrorWhenReadBegun(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ErrorWhileRead,
            LogLevel.Debug,
            "Read",
            EventName = "ErrorWhileRead"
        )]
        partial public static void ErrorWhileRead(ILogger logger, Exception exception);
    }
}
