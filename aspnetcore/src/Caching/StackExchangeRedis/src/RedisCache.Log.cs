// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Caching.StackExchangeRedis;

partial public class RedisCache
{
    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Warning,
            "Could not determine the Redis server version. Falling back to use HMSET command instead of HSET.",
            EventName = "CouldNotDetermineServerVersion"
        )]
        partial public static void CouldNotDetermineServerVersion(
            ILogger logger,
            Exception exception
        );
    }
}
