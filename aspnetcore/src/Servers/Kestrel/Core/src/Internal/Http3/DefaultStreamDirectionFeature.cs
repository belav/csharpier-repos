using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http3;

internal sealed class DefaultStreamDirectionFeature : IStreamDirectionFeature
{
    public DefaultStreamDirectionFeature(bool canRead, bool canWrite)
    {
        CanRead = canRead;
        CanWrite = canWrite;
    }

    public bool CanRead { get; }

    public bool CanWrite { get; }
}
