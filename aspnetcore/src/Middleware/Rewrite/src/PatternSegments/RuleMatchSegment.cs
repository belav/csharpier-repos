using System.Diagnostics;

namespace Microsoft.AspNetCore.Rewrite.PatternSegments;

internal sealed class RuleMatchSegment : PatternSegment
{
    private readonly int _index;

    public RuleMatchSegment(int index)
    {
        _index = index;
    }

    public override string? Evaluate(
        RewriteContext context,
        BackReferenceCollection? ruleBackReferences,
        BackReferenceCollection? conditionBackReferences
    )
    {
        Debug.Assert(ruleBackReferences != null);
        return ruleBackReferences[_index];
    }
}
