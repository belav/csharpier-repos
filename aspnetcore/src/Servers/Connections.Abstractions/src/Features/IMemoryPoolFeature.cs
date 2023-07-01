using System.Buffers;

namespace Microsoft.AspNetCore.Connections.Features;

/// <summary>
/// The <see cref="MemoryPool{T}"/> used by the connection.
/// </summary>
public interface IMemoryPoolFeature
{
    /// <summary>
    /// Gets the <see cref="MemoryPool{T}"/> used by the connection.
    /// </summary>
    MemoryPool<byte> MemoryPool { get; }
}
