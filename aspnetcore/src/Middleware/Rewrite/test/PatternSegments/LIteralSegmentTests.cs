using Microsoft.AspNetCore.Rewrite.PatternSegments;

namespace Microsoft.AspNetCore.Rewrite.Tests.PatternSegments;

public class LiteralSegmentTests
{
    [Fact]
    public void LiteralSegment_AssertSegmentIsCorrect()
    {
        // Arrange
        var segement = new LiteralSegment("foo");

        // Act
        var results = segement.Evaluate(null, null, null);

        // Assert
        Assert.Equal("foo", results);
    }
}
