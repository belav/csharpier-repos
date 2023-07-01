using System.Globalization;

namespace Microsoft.AspNetCore.Rewrite.PatternSegments;

internal sealed class LocalPortSegment : PatternSegment
{
    public override string? Evaluate(
        RewriteContext context,
        BackReferenceCollection? ruleBackReferences,
        BackReferenceCollection? conditionBackReferences
    )
    {
        return context.HttpContext.Connection.LocalPort.ToString(CultureInfo.InvariantCulture);
    }
}
