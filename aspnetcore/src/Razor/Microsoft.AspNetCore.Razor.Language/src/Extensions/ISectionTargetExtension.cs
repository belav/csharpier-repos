using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language.Extensions;

public interface ISectionTargetExtension : ICodeTargetExtension
{
    void WriteSection(CodeRenderingContext context, SectionIntermediateNode node);
}
