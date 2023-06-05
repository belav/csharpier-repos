// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal class UrlGroup
{
    partial private static class Log
    {
        [LoggerMessage(
            LoggerEventIds.CloseUrlGroupError,
            LogLevel.Error,
            "HttpCloseUrlGroup; Result: {StatusCode}",
            EventName = "CloseUrlGroupError"
        )]
        partial public static void CloseUrlGroupError(ILogger logger, uint statusCode);

        [LoggerMessage(
            LoggerEventIds.RegisteringPrefix,
            LogLevel.Debug,
            "Listening on prefix: {UriPrefix}",
            EventName = "RegisteringPrefix"
        )]
        partial public static void RegisteringPrefix(ILogger logger, string uriPrefix);

        [LoggerMessage(
            LoggerEventIds.SetUrlPropertyError,
            LogLevel.Error,
            "SetUrlGroupProperty",
            EventName = "SetUrlPropertyError"
        )]
        partial public static void SetUrlPropertyError(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.UnregisteringPrefix,
            LogLevel.Information,
            "Stop listening on prefix: {UriPrefix}",
            EventName = "UnregisteringPrefix"
        )]
        partial public static void UnregisteringPrefix(ILogger logger, string uriPrefix);
    }
}
