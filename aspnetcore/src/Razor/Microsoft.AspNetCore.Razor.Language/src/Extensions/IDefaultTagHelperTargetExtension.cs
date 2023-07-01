using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language.Extensions;

public interface IDefaultTagHelperTargetExtension : ICodeTargetExtension
{
    void WriteTagHelperBody(
        CodeRenderingContext context,
        DefaultTagHelperBodyIntermediateNode node
    );

    void WriteTagHelperCreate(
        CodeRenderingContext context,
        DefaultTagHelperCreateIntermediateNode node
    );

    void WriteTagHelperExecute(
        CodeRenderingContext context,
        DefaultTagHelperExecuteIntermediateNode node
    );

    void WriteTagHelperHtmlAttribute(
        CodeRenderingContext context,
        DefaultTagHelperHtmlAttributeIntermediateNode node
    );

    void WriteTagHelperProperty(
        CodeRenderingContext context,
        DefaultTagHelperPropertyIntermediateNode node
    );

    void WriteTagHelperRuntime(
        CodeRenderingContext context,
        DefaultTagHelperRuntimeIntermediateNode node
    );
}
