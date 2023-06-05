// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal class RequestContext
{
    partial private static class Log
    {
        [LoggerMessage(
            LoggerEventIds.AbortError,
            LogLevel.Debug,
            "Abort",
            EventName = "AbortError"
        )]
        partial public static void AbortError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ChannelBindingNeedsHttps,
            LogLevel.Debug,
            "TryGetChannelBinding; Channel binding requires HTTPS.",
            EventName = "ChannelBindingNeedsHttps"
        )]
        partial public static void ChannelBindingNeedsHttps(ILogger logger);

        [LoggerMessage(
            LoggerEventIds.ChannelBindingRetrieved,
            LogLevel.Debug,
            "Channel binding retrieved.",
            EventName = "ChannelBindingRetrieved"
        )]
        partial public static void ChannelBindingRetrieved(ILogger logger);
    }
}
