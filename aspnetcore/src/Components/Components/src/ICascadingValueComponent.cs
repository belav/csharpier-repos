using Microsoft.AspNetCore.Components.Rendering;

namespace Microsoft.AspNetCore.Components;

internal interface ICascadingValueComponent
{
    // This interface exists only so that CascadingParameterState has a way
    // to work with all CascadingValue<T> types regardless of T.

    bool CanSupplyValue(Type valueType, string? valueName);

    object? CurrentValue { get; }

    bool CurrentValueIsFixed { get; }

    void Subscribe(ComponentState subscriber);

    void Unsubscribe(ComponentState subscriber);
}
