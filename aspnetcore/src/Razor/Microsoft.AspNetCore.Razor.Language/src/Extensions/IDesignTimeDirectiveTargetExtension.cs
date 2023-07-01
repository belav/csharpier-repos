using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language.Extensions;

internal interface IDesignTimeDirectiveTargetExtension : ICodeTargetExtension
{
    void WriteDesignTimeDirective(
        CodeRenderingContext context,
        DesignTimeDirectiveIntermediateNode node
    );
}
