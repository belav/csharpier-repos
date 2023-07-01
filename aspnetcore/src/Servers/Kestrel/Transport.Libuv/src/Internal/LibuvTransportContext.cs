using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal
{
    internal class LibuvTransportContext
    {
#pragma warning disable CS0618
        public LibuvTransportOptions Options { get; set; }
#pragma warning restore CS0618

        public IHostApplicationLifetime AppLifetime { get; set; }

        public ILibuvTrace Log { get; set; }
    }
}
