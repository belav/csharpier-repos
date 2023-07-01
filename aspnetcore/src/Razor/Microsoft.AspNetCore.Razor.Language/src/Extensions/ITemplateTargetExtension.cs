using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language.Extensions;

public interface ITemplateTargetExtension : ICodeTargetExtension
{
    void WriteTemplate(CodeRenderingContext context, TemplateIntermediateNode node);
}
