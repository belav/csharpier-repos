using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.ResponseCaching;

internal sealed class CachedVaryByRules : IResponseCacheEntry
{
    public string VaryByKeyPrefix { get; set; } = default!;

    public StringValues Headers { get; set; }

    public StringValues QueryKeys { get; set; }
}
