// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

partial internal class ClientCertLoader
{
    partial private static class Log
    {
        [LoggerMessage(
            LoggerEventIds.ChannelBindingMissing,
            LogLevel.Error,
            "GetChannelBindingFromTls",
            EventName = "ChannelBindingMissing"
        )]
        partial public static void ChannelBindingMissing(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ChannelBindingUnsupported,
            LogLevel.Error,
            "GetChannelBindingFromTls; Channel binding is not supported.",
            EventName = "ChannelBindingUnsupported"
        )]
        partial public static void ChannelBindingUnsupported(ILogger logger);
    }
}
