using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language.Extensions;

internal interface IPreallocatedAttributeTargetExtension : ICodeTargetExtension
{
    void WriteTagHelperHtmlAttribute(
        CodeRenderingContext context,
        PreallocatedTagHelperHtmlAttributeIntermediateNode node
    );

    void WriteTagHelperHtmlAttributeValue(
        CodeRenderingContext context,
        PreallocatedTagHelperHtmlAttributeValueIntermediateNode node
    );

    void WriteTagHelperProperty(
        CodeRenderingContext context,
        PreallocatedTagHelperPropertyIntermediateNode node
    );

    void WriteTagHelperPropertyValue(
        CodeRenderingContext context,
        PreallocatedTagHelperPropertyValueIntermediateNode node
    );
}
