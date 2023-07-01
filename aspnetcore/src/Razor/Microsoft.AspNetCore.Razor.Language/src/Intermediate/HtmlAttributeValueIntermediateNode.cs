using System;

namespace Microsoft.AspNetCore.Razor.Language.Intermediate;

public sealed class HtmlAttributeValueIntermediateNode : IntermediateNode
{
    public override IntermediateNodeCollection Children { get; } = new IntermediateNodeCollection();

    public string Prefix { get; set; }

    public override void Accept(IntermediateNodeVisitor visitor)
    {
        if (visitor == null)
        {
            throw new ArgumentNullException(nameof(visitor));
        }

        visitor.VisitHtmlAttributeValue(this);
    }

    public override void FormatNode(IntermediateNodeFormatter formatter)
    {
        formatter.WriteChildren(Children);

        formatter.WriteProperty(nameof(Prefix), Prefix);
    }
}
