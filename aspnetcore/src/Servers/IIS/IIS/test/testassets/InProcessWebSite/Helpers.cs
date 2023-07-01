using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace TestSite;

public static class Helpers
{
    internal static bool? CanHaveBody(this HttpRequest request)
    {
#if FORWARDCOMPAT
        return null;
#else
        return request.HttpContext.Features.Get<IHttpRequestBodyDetectionFeature>()?.CanHaveBody;
#endif
    }
}
