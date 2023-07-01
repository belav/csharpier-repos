using System;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;

[Flags]
internal enum Http2ContinuationFrameFlags : byte
{
    NONE = 0x0,
    END_HEADERS = 0x4,
}
