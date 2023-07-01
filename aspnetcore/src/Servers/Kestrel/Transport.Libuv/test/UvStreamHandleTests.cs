using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Tests.TestHelpers;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Tests
{
    public class UvStreamHandleTests
    {
        [Fact]
        public void ReadStopIsIdempotent()
        {
            var libuvTrace = new LibuvTrace(new TestApplicationErrorLogger());

            using (var uvLoopHandle = new UvLoopHandle(libuvTrace))
            using (var uvTcpHandle = new UvTcpHandle(libuvTrace))
            {
                uvLoopHandle.Init(new MockLibuv());
                uvTcpHandle.Init(uvLoopHandle, null);

                UvStreamHandle uvStreamHandle = uvTcpHandle;
                uvStreamHandle.ReadStop();
                uvStreamHandle.ReadStop();
            }
        }
    }
}
