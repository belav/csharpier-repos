using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Filters;

internal sealed class ControllerViewDataAttributeFilterFactory : IFilterFactory
{
    public ControllerViewDataAttributeFilterFactory(IReadOnlyList<LifecycleProperty> properties)
    {
        Properties = properties;
    }

    public IReadOnlyList<LifecycleProperty> Properties { get; }

    // ControllerViewDataAttributeFilter is stateful and cannot be reused.
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return new ControllerViewDataAttributeFilter(Properties);
    }
}
