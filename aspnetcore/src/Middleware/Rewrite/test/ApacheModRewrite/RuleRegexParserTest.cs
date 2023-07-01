using Microsoft.AspNetCore.Rewrite.ApacheModRewrite;

namespace Microsoft.AspNetCore.Rewrite.Tests.ModRewrite;

public class RuleRegexParserTest
{
    [Fact]
    public void RuleRegexParser_ShouldThrowOnNull()
    {
        Assert.Throws<FormatException>(() => RuleRegexParser.ParseRuleRegex(null));
    }

    [Fact]
    public void RuleRegexParser_ShouldThrowOnEmpty()
    {
        Assert.Throws<FormatException>(() => RuleRegexParser.ParseRuleRegex(string.Empty));
    }

    [Fact]
    public void RuleRegexParser_RegularRegexExpression()
    {
        var results = RuleRegexParser.ParseRuleRegex("(.*)");
        Assert.False(results.Invert);
        Assert.Equal("(.*)", results.Operand);
    }
}
