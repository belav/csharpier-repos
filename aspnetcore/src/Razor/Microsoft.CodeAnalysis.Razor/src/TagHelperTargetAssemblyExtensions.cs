using Microsoft.AspNetCore.Razor.Language;

namespace Microsoft.CodeAnalysis.Razor;

internal static class TagHelperTargetAssemblyExtensions
{
    private static readonly object TargetAssemblyKey = new object();

    public static IAssemblySymbol? GetTargetAssembly(this ItemCollection items)
    {
        if (items.Count == 0 || items[TargetAssemblyKey] is not IAssemblySymbol symbol)
        {
            return null;
        }

        return symbol;
    }

    public static void SetTargetAssembly(this ItemCollection items, IAssemblySymbol symbol)
    {
        items[TargetAssemblyKey] = symbol;
    }
}
