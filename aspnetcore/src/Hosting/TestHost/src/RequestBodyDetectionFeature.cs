using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.AspNetCore.TestHost;

internal sealed class RequestBodyDetectionFeature : IHttpRequestBodyDetectionFeature
{
    public RequestBodyDetectionFeature(bool canHaveBody)
    {
        CanHaveBody = canHaveBody;
    }

    public bool CanHaveBody { get; }
}
