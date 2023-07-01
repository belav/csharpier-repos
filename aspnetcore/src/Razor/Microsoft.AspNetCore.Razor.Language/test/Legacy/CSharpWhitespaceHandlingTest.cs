using System;
using Xunit;

namespace Microsoft.AspNetCore.Razor.Language.Legacy;

public class CSharpWhitespaceHandlingTest : ParserTestBase
{
    [Fact]
    public void StmtBlockDoesNotAcceptTrailingNewlineIfTheyAreSignificantToAncestor()
    {
        ParseDocumentTest("@{@: @if (true) { }" + Environment.NewLine + "}");
    }
}
