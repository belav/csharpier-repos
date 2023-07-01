using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.AspNetCore.Http;

internal static class RequestExtensions
{
    internal static bool? CanHaveBody(this HttpRequest request)
    {
        return request.HttpContext.Features.Get<IHttpRequestBodyDetectionFeature>()?.CanHaveBody;
    }
}
