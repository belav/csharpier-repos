using System.Linq;

namespace Microsoft.AspNetCore.Razor.Language;

internal class DefaultRazorOptimizationPhase : RazorEnginePhaseBase, IRazorOptimizationPhase
{
    public IRazorOptimizationPass[] Passes { get; private set; }

    protected override void OnIntialized()
    {
        Passes = Engine.Features.OfType<IRazorOptimizationPass>().OrderBy(p => p.Order).ToArray();
    }

    protected override void ExecuteCore(RazorCodeDocument codeDocument)
    {
        var documentNode = codeDocument.GetDocumentIntermediateNode();
        ThrowForMissingDocumentDependency(documentNode);

        foreach (var pass in Passes)
        {
            pass.Execute(codeDocument, documentNode);
        }

        codeDocument.SetDocumentIntermediateNode(documentNode);
    }
}
