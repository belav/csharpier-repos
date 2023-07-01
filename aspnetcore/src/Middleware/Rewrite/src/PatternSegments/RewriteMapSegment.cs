using Microsoft.AspNetCore.Rewrite.IISUrlRewrite;

namespace Microsoft.AspNetCore.Rewrite.PatternSegments;

internal sealed class RewriteMapSegment : PatternSegment
{
    private readonly IISRewriteMap _rewriteMap;
    private readonly Pattern _pattern;

    public RewriteMapSegment(IISRewriteMap rewriteMap, Pattern pattern)
    {
        _rewriteMap = rewriteMap;
        _pattern = pattern;
    }

    public override string? Evaluate(
        RewriteContext context,
        BackReferenceCollection? ruleBackReferences,
        BackReferenceCollection? conditionBackReferences
    )
    {
        var key = _pattern
            .Evaluate(context, ruleBackReferences, conditionBackReferences)
            .ToLowerInvariant();
        return _rewriteMap[key];
    }
}
