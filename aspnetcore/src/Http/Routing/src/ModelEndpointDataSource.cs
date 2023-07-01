using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Routing;

internal sealed class ModelEndpointDataSource : EndpointDataSource
{
    private readonly List<DefaultEndpointConventionBuilder> _endpointConventionBuilders;

    public ModelEndpointDataSource()
    {
        _endpointConventionBuilders = new List<DefaultEndpointConventionBuilder>();
    }

    public IEndpointConventionBuilder AddEndpointBuilder(EndpointBuilder endpointBuilder)
    {
        var builder = new DefaultEndpointConventionBuilder(endpointBuilder);
        _endpointConventionBuilders.Add(builder);

        return builder;
    }

    public override IChangeToken GetChangeToken()
    {
        return NullChangeToken.Singleton;
    }

    public override IReadOnlyList<Endpoint> Endpoints =>
        _endpointConventionBuilders.Select(e => e.Build()).ToArray();

    // for testing
    internal IEnumerable<EndpointBuilder> EndpointBuilders =>
        _endpointConventionBuilders.Select(b => b.EndpointBuilder);
}
