using System.Linq;

namespace Microsoft.AspNetCore.Razor.Language;

internal class DefaultRazorDocumentClassifierPhase
    : RazorEnginePhaseBase,
        IRazorDocumentClassifierPhase
{
    public IRazorDocumentClassifierPass[] Passes { get; private set; }

    protected override void OnIntialized()
    {
        Passes = Engine.Features
            .OfType<IRazorDocumentClassifierPass>()
            .OrderBy(p => p.Order)
            .ToArray();
    }

    protected override void ExecuteCore(RazorCodeDocument codeDocument)
    {
        var irDocument = codeDocument.GetDocumentIntermediateNode();
        ThrowForMissingDocumentDependency(irDocument);

        foreach (var pass in Passes)
        {
            pass.Execute(codeDocument, irDocument);
        }

        codeDocument.SetDocumentIntermediateNode(irDocument);
    }
}
