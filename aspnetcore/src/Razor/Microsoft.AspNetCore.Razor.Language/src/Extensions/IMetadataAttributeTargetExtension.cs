using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language.Extensions;

internal interface IMetadataAttributeTargetExtension : ICodeTargetExtension
{
    void WriteRazorCompiledItemAttribute(
        CodeRenderingContext context,
        RazorCompiledItemAttributeIntermediateNode node
    );

    void WriteRazorSourceChecksumAttribute(
        CodeRenderingContext context,
        RazorSourceChecksumAttributeIntermediateNode node
    );

    void WriteRazorCompiledItemMetadataAttribute(
        CodeRenderingContext context,
        RazorCompiledItemMetadataAttributeIntermediateNode node
    );
}
