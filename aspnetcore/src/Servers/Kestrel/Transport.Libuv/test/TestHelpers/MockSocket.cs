using System;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Tests.TestHelpers
{
    class MockSocket : UvStreamHandle
    {
        public MockSocket(LibuvFunctions uv, int threadId, ILibuvTrace logger)
            : base(logger)
        {
            CreateMemory(uv, threadId, IntPtr.Size);
        }

        protected override bool ReleaseHandle()
        {
            DestroyMemory(handle);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
