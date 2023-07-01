using System;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Experimental.Quic.Internal
{
    internal interface IQuicTrace : ILogger
    {
        void AcceptedConnection(BaseConnectionContext connection);
        void AcceptedStream(QuicStreamContext streamContext);
        void ConnectionError(BaseConnectionContext connection, Exception ex);
        void StreamError(QuicStreamContext streamContext, Exception ex);
        void StreamPause(QuicStreamContext streamContext);
        void StreamResume(QuicStreamContext streamContext);
        void StreamShutdownWrite(QuicStreamContext streamContext, string reason);
        void StreamAbort(QuicStreamContext streamContext, string reason);
    }
}
