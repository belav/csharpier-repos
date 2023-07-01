using System.IO.Pipes;

namespace Microsoft.AspNetCore.Connections.Features;

/// <summary>
/// Provides access to the connection's underlying <see cref="NamedPipeServerStream"/>.
/// </summary>
public interface IConnectionNamedPipeFeature
{
    /// <summary>
    /// Gets the underlying <see cref="NamedPipeServerStream"/>.
    /// </summary>
    NamedPipeServerStream NamedPipe { get; }
}
