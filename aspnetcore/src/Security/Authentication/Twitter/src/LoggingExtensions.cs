// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(2, LogLevel.Debug, "ObtainAccessToken", EventName = "ObtainAccessToken")]
    partial public static void ObtainAccessToken(this ILogger logger);

    [LoggerMessage(1, LogLevel.Debug, "ObtainRequestToken", EventName = "ObtainRequestToken")]
    partial public static void ObtainRequestToken(this ILogger logger);

    [LoggerMessage(3, LogLevel.Debug, "RetrieveUserDetails", EventName = "RetrieveUserDetails")]
    partial public static void RetrieveUserDetails(this ILogger logger);
}
