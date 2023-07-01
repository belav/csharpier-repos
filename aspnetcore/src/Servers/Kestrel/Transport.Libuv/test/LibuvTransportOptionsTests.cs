using System;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Tests
{
    public class LibuvTransportOptionsTests
    {
        [Fact]
        public void SetThreadCountUsingProcessorCount()
        {
            // Ideally we'd mock Environment.ProcessorCount to test edge cases.
            var expected = Clamp(Environment.ProcessorCount >> 1, 1, 16);

#pragma warning disable CS0618
            var information = new LibuvTransportOptions();

            Assert.Equal(expected, information.ThreadCount);
#pragma warning restore CS0618
        }

        private static int Clamp(int value, int min, int max)
        {
            return value < min
                ? min
                : value > max
                    ? max
                    : value;
        }
    }
}
