using System.Net.Http;
using NUnit.Framework;

namespace MonoTests.System.Net.Http.Headers
{
    [TestFixture]
    public class HttpRequestHeadersTest
    {
        [Test]
        public void AccessHostAfterAdding()
        {
            var requestMessage = new HttpRequestMessage();
            requestMessage.Headers.TryAddWithoutValidation("Host", "MyHost:90");

            Assert.AreEqual("MyHost:90", requestMessage.Headers.Host);
        }
    }
}
