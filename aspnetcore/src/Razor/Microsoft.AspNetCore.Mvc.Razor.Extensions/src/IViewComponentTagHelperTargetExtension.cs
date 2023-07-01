using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions;

public interface IViewComponentTagHelperTargetExtension : ICodeTargetExtension
{
    void WriteViewComponentTagHelper(
        CodeRenderingContext context,
        ViewComponentTagHelperIntermediateNode node
    );
}
