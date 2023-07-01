using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Sockets.BindTests;

public class SocketTransportFactoryTests
{
    [Fact]
    public async Task ThrowsNotImplementedExceptionWhenBindingToUriEndPoint()
    {
        var socketTransportFactory = new SocketTransportFactory(
            Options.Create(new SocketTransportOptions()),
            new LoggerFactory()
        );
        await Assert.ThrowsAsync<NotImplementedException>(
            async () =>
                await socketTransportFactory.BindAsync(
                    new UriEndPoint(new Uri("http://127.0.0.1:5554"))
                )
        );
    }
}
