using System.Text.Encodings.Web;
using Microsoft.Extensions.ObjectPool;

namespace Microsoft.AspNetCore.Routing;

internal sealed class UriBuilderContextPooledObjectPolicy : IPooledObjectPolicy<UriBuildingContext>
{
    public UriBuildingContext Create()
    {
        return new UriBuildingContext(UrlEncoder.Default);
    }

    public bool Return(UriBuildingContext obj)
    {
        obj.Clear();
        return true;
    }
}
