using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Connections.Features;

/// <summary>
/// Represents the close action for a stream.
/// </summary>
public interface IStreamClosedFeature
{
    /// <summary>
    /// Registers a callback to be invoked when a stream is closed.
    /// If the stream is already in a closed state, the callback will be run immediately.
    /// </summary>
    /// <param name="callback">The callback to invoke after the stream is closed.</param>
    /// <param name="state">The state to pass into the callback.</param>
    void OnClosed(Action<object?> callback, object? state);
}
