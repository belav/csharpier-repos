using System.Security.Claims;

namespace Microsoft.AspNetCore.Components.Server.Circuits;

internal interface ICircuitFactory
{
    ValueTask<CircuitHost> CreateCircuitHostAsync(
        IReadOnlyList<ComponentDescriptor> components,
        CircuitClientProxy client,
        string baseUri,
        string uri,
        ClaimsPrincipal user,
        IPersistentComponentStateStore store
    );
}
