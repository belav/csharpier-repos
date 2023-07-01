using System;

namespace Microsoft.AspNetCore.Internal;

/// <summary>
/// An empty scope without any logic
/// </summary>
internal sealed class NullScope : IDisposable
{
    public static NullScope Instance { get; } = new NullScope();

    private NullScope() { }

    /// <inheritdoc />
    public void Dispose() { }
}
