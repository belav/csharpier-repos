// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "HandleChallenge with Location: {Location}; and Set-Cookie: {Cookie}.",
        EventName = "HandleChallenge"
    )]
    partial public static void HandleChallenge(this ILogger logger, string location, string cookie);
}
