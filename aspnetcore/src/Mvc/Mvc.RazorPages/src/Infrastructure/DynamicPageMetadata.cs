using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

internal sealed class DynamicPageMetadata : IDynamicEndpointMetadata
{
    public DynamicPageMetadata(RouteValueDictionary values)
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
