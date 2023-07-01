using System.Net.Sockets;

namespace Microsoft.AspNetCore.Connections.Features;

/// <summary>
/// Provides access to the connection's underlying <see cref="Socket"/>.
/// </summary>
public interface IConnectionSocketFeature
{
    /// <summary>
    /// Gets the underlying <see cref="Socket"/>.
    /// </summary>
    Socket Socket { get; }
}
