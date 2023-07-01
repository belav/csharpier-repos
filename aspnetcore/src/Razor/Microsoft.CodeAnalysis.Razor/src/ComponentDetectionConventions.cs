using System;

namespace Microsoft.CodeAnalysis.Razor;

internal static class ComponentDetectionConventions
{
    public static bool IsComponent(INamedTypeSymbol symbol, INamedTypeSymbol icomponentSymbol)
    {
        if (symbol is null)
        {
            throw new ArgumentNullException(nameof(symbol));
        }

        if (icomponentSymbol is null)
        {
            throw new ArgumentNullException(nameof(icomponentSymbol));
        }

        return symbol.DeclaredAccessibility == Accessibility.Public
            && !symbol.IsAbstract
            && symbol.AllInterfaces.Contains(icomponentSymbol);
    }
}
