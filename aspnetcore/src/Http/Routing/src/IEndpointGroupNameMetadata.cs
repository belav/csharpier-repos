using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing;

/// <summary>
/// Defines a contract used to specify an endpoint group name in <see cref="Endpoint.Metadata"/>.
/// </summary>
public interface IEndpointGroupNameMetadata
{
    /// <summary>
    /// Gets the endpoint group name.
    /// </summary>
    string EndpointGroupName { get; }
}
