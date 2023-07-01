using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Matching;

namespace Microsoft.AspNetCore.Routing.TestObjects;

internal class TestMatcher : Matcher
{
    private readonly bool _isHandled;

    public TestMatcher(bool isHandled)
    {
        _isHandled = isHandled;
    }

    public override Task MatchAsync(HttpContext httpContext)
    {
        if (_isHandled)
        {
            httpContext.Request.RouteValues = new RouteValueDictionary(
                new { controller = "Home", action = "Index" }
            );
            httpContext.SetEndpoint(
                new Endpoint(
                    TestConstants.EmptyRequestDelegate,
                    EndpointMetadataCollection.Empty,
                    "Test endpoint"
                )
            );
        }

        return Task.CompletedTask;
    }
}
