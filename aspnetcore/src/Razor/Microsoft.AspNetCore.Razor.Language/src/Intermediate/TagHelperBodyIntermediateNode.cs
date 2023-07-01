using System;

namespace Microsoft.AspNetCore.Razor.Language.Intermediate;

public sealed class TagHelperBodyIntermediateNode : IntermediateNode
{
    public override IntermediateNodeCollection Children { get; } = new IntermediateNodeCollection();

    public override void Accept(IntermediateNodeVisitor visitor)
    {
        if (visitor == null)
        {
            throw new ArgumentNullException(nameof(visitor));
        }

        visitor.VisitTagHelperBody(this);
    }
}
