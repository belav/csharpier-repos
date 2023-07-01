using Xunit;

namespace Microsoft.AspNetCore.Razor.Language.Legacy;

public class CSharpReservedWordsTest : ParserTestBase
{
    [Fact]
    public void ReservedWord()
    {
        ParseDocumentTest("@namespace");
    }

    [Fact]
    private void ReservedWordIsCaseSensitive()
    {
        ParseDocumentTest("@NameSpace");
    }
}
