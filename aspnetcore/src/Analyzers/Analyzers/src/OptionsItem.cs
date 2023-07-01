using Microsoft.CodeAnalysis;

namespace Microsoft.AspNetCore.Analyzers;

internal sealed class OptionsItem
{
    public OptionsItem(IPropertySymbol property, object constantValue)
    {
        Property = property;
        ConstantValue = constantValue;
    }

    public INamedTypeSymbol OptionsType => Property.ContainingType;

    public IPropertySymbol Property { get; }

    public object ConstantValue { get; }
}
