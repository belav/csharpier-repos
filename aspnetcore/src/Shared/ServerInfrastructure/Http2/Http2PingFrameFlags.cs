using System;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;

[Flags]
internal enum Http2PingFrameFlags : byte
{
    NONE = 0x0,
    ACK = 0x1
}
