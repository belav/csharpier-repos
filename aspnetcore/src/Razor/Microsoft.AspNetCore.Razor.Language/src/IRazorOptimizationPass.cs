using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Microsoft.AspNetCore.Razor.Language;

public interface IRazorOptimizationPass : IRazorEngineFeature
{
    int Order { get; }

    void Execute(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode);
}
