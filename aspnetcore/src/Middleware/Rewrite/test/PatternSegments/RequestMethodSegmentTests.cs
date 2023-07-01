using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.PatternSegments;

namespace Microsoft.AspNetCore.Rewrite.Tests.PatternSegments;

public class RequestMethodSegmentTests
{
    [Fact]
    public void RequestMethod_AssertSegmentIsCorrect()
    {
        // Arrange
        var segement = new RequestMethodSegment();
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.Request.Method = HttpMethods.Get;
        // Act
        var results = segement.Evaluate(context, null, null);

        // Assert
        Assert.Equal(HttpMethods.Get, results);
    }
}
