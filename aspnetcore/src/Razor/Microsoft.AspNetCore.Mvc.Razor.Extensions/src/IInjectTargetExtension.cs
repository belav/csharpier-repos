using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions;

public interface IInjectTargetExtension : ICodeTargetExtension
{
    void WriteInjectProperty(CodeRenderingContext context, InjectIntermediateNode node);
}
