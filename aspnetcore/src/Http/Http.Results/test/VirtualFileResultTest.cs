using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Internal;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http.Result;

public class VirtualFileResultTest : VirtualFileResultTestBase
{
    protected override Task ExecuteAsync(
        HttpContext httpContext,
        string path,
        string contentType,
        DateTimeOffset? lastModified = null,
        EntityTagHeaderValue entityTag = null,
        bool enableRangeProcessing = false
    )
    {
        var result = new VirtualFileResult(path, contentType)
        {
            LastModified = lastModified,
            EntityTag = entityTag,
            EnableRangeProcessing = enableRangeProcessing,
        };

        return result.ExecuteAsync(httpContext);
    }
}
