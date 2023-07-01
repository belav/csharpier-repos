using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.AspNetCore.Mvc.Analyzers;

public class ViewFeaturesAnalyzerContext
{
#pragma warning disable RS1012 // Start action has no registered actions.
    public ViewFeaturesAnalyzerContext(CompilationStartAnalysisContext context)
#pragma warning restore RS1012 // Start action has no registered actions.
    {
        Context = context;
        HtmlHelperType = GetType(SymbolNames.IHtmlHelperType);
        HtmlHelperPartialExtensionsType = GetType(SymbolNames.HtmlHelperPartialExtensionsType);
    }

    public CompilationStartAnalysisContext Context { get; }

    public INamedTypeSymbol HtmlHelperType { get; }

    public INamedTypeSymbol HtmlHelperPartialExtensionsType { get; }

    private INamedTypeSymbol GetType(string name) =>
        Context.Compilation.GetTypeByMetadataName(name);

    public bool IsHtmlHelperExtensionMethod(IMethodSymbol method)
    {
        if (!method.IsExtensionMethod)
        {
            return false;
        }

        if (
            !SymbolEqualityComparer.Default.Equals(
                method.ContainingType,
                HtmlHelperPartialExtensionsType
            )
        )
        {
            return false;
        }

        return true;
    }
}
