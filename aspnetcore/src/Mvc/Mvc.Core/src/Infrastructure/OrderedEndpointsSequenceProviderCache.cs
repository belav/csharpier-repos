using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.Infrastructure;

internal sealed class OrderedEndpointsSequenceProviderCache
{
    private readonly ConcurrentDictionary<
        IEndpointRouteBuilder,
        OrderedEndpointsSequenceProvider
    > _sequenceProviderCache = new();

    public OrderedEndpointsSequenceProvider GetOrCreateOrderedEndpointsSequenceProvider(
        IEndpointRouteBuilder endpoints
    )
    {
        return _sequenceProviderCache.GetOrAdd(endpoints, new OrderedEndpointsSequenceProvider());
    }
}
