using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Caching.StackExchangeRedis;

public partial class RedisCache
{
    private static partial class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Warning,
            "Could not determine the Redis server version. Falling back to use HMSET command instead of HSET.",
            EventName = "CouldNotDetermineServerVersion"
        )]
        public static partial void CouldNotDetermineServerVersion(
            ILogger logger,
            Exception exception
        );
    }
}
