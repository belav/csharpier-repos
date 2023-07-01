using System;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Internal;

internal interface IFileResultLogger
{
    void IfUnmodifiedSincePreconditionFailed(
        DateTimeOffset? lastModified,
        DateTimeOffset? ifUnmodifiedSinceDate
    );

    void IfMatchPreconditionFailed(EntityTagHeaderValue etag);

    void NotEnabledForRangeProcessing();
}
