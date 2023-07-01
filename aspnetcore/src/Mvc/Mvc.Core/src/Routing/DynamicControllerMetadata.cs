using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.Routing;

internal sealed class DynamicControllerMetadata : IDynamicEndpointMetadata
{
    public DynamicControllerMetadata(RouteValueDictionary values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        Values = values;
    }

    public bool IsDynamic => true;

    public RouteValueDictionary Values { get; }
}
