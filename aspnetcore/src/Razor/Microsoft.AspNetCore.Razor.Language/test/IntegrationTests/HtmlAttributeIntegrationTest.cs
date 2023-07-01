using Xunit;

namespace Microsoft.AspNetCore.Razor.Language.IntegrationTests;

public class HtmlAttributeIntegrationTest : IntegrationTestBase
{
    [Fact]
    public void HtmlWithDataDashAttribute()
    {
        // Arrange
        var projectItem = CreateProjectItemFromFile();

        // Act
        var compiled = CompileToCSharp(projectItem);

        // Assert
        AssertDocumentNodeMatchesBaseline(compiled.CodeDocument.GetDocumentIntermediateNode());
    }

    [Fact]
    public void HtmlWithConditionalAttribute()
    {
        // Arrange
        var projectItem = CreateProjectItemFromFile();

        // Act
        var compiled = CompileToCSharp(projectItem);

        // Assert
        AssertDocumentNodeMatchesBaseline(compiled.CodeDocument.GetDocumentIntermediateNode());
    }
}
