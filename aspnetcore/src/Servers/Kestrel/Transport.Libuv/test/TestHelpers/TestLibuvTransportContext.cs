using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Tests.TestHelpers
{
    internal class TestLibuvTransportContext : LibuvTransportContext
    {
        public TestLibuvTransportContext()
        {
            var logger = new TestApplicationErrorLogger();

            AppLifetime = new LifetimeNotImplemented();
            Log = new LibuvTrace(logger);
#pragma warning disable CS0618
            Options = new LibuvTransportOptions { ThreadCount = 1 };
#pragma warning restore CS0618
        }
    }
}
