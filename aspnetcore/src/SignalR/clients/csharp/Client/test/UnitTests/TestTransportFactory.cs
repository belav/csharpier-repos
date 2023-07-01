using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client.Internal;

namespace Microsoft.AspNetCore.SignalR.Client.Tests;

internal class TestTransportFactory : ITransportFactory
{
    private readonly ITransport _transport;

    public TestTransportFactory(ITransport transport)
    {
        _transport = transport;
    }

    public ITransport CreateTransport(HttpTransportType availableServerTransports)
    {
        return _transport;
    }
}
