using System;

namespace Microsoft.Extensions.Caching.SqlServer;

public class CacheItemInfo
{
    public string Id { get; set; }

    public byte[] Value { get; set; }

    public DateTimeOffset ExpiresAtTime { get; set; }

    public TimeSpan? SlidingExpirationInSeconds { get; set; }

    public DateTimeOffset? AbsoluteExpiration { get; set; }
}
