using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

namespace Microsoft.AspNetCore.Testing
{
    internal class TestKestrelTrace : KestrelTrace
    {
        public TestKestrelTrace()
            : this(new TestApplicationErrorLogger()) { }

        public TestKestrelTrace(TestApplicationErrorLogger testLogger)
            : base(testLogger)
        {
            Logger = testLogger;
        }

        public TestApplicationErrorLogger Logger { get; private set; }
    }
}
