using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Quic.Internal;

internal sealed class QuicTransportContext
{
    public QuicTransportContext(ILogger log, QuicTransportOptions options)
    {
        Log = log;
        Options = options;
    }

    public ILogger Log { get; }
    public QuicTransportOptions Options { get; }
}
