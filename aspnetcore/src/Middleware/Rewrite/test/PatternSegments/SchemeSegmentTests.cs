using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.PatternSegments;

namespace Microsoft.AspNetCore.Rewrite.Tests.PatternSegments;

public class SchemeSegmentTests
{
    [Fact]
    public void SchemeSegment_AssertSegmentIsCorrect()
    {
        // Arrange
        var segement = new SchemeSegment();
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.Request.Scheme = "http";
        // Act
        var results = segement.Evaluate(context, null, null);

        // Assert
        Assert.Equal("http", results);
    }
}
