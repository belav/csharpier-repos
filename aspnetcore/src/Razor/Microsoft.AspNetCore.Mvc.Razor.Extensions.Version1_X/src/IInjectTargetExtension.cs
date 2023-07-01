using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions.Version1_X;

public interface IInjectTargetExtension : ICodeTargetExtension
{
    void WriteInjectProperty(CodeRenderingContext context, InjectIntermediateNode node);
}
