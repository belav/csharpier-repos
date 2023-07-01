using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Microsoft.AspNetCore.Razor.Language.Components;

internal sealed class RouteAttributeExtensionNode : ExtensionIntermediateNode
{
    public RouteAttributeExtensionNode(StringSegment template)
    {
        Template = template;
    }

    public StringSegment Template { get; }

    public override IntermediateNodeCollection Children => IntermediateNodeCollection.ReadOnly;

    public override void Accept(IntermediateNodeVisitor visitor) =>
        AcceptExtensionNode(this, visitor);

    public override void WriteNode(CodeTarget target, CodeRenderingContext context)
    {
        context.CodeWriter.Write("[");
        context.CodeWriter.Write(ComponentsApi.RouteAttribute.FullTypeName);
        context.CodeWriter.Write("(\"");
        context.CodeWriter.Write(Template);
        context.CodeWriter.Write("\")");
        context.CodeWriter.Write("]");
        context.CodeWriter.WriteLine();
    }
}
