using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.ResponseCaching;

internal sealed class MemoryCachedResponse
{
    public DateTimeOffset Created { get; set; }

    public int StatusCode { get; set; }

    public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

    public CachedResponseBody Body { get; set; } = default!;
}
