using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Rewrite.PatternSegments;

namespace Microsoft.AspNetCore.Rewrite.Tests.PatternSegments;

public class ServerProtocolSegmentTests
{
    [Fact]
    public void ServerProtocol_AssertSegmentIsCorrect()
    {
        // Arrange
        var segement = new ServerProtocolSegment();
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.Features.Set<IHttpRequestFeature>(
            new HttpRequestFeature { Protocol = "http" }
        );

        // Act
        var results = segement.Evaluate(context, null, null);

        // Assert
        Assert.Equal("http", results);
    }
}
