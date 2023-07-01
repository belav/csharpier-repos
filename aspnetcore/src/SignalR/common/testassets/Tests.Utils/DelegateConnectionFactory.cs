using System.Net;
using Microsoft.AspNetCore.Connections;

namespace Microsoft.AspNetCore.SignalR.Tests;

public class DelegateConnectionFactory : IConnectionFactory
{
    private readonly Func<EndPoint, ValueTask<ConnectionContext>> _connectDelegate;

    // We have no tests that use the CancellationToken. When we do, we can add it to the delegate. This is test code.
    public DelegateConnectionFactory(Func<EndPoint, ValueTask<ConnectionContext>> connectDelegate)
    {
        _connectDelegate = connectDelegate;
    }

    public ValueTask<ConnectionContext> ConnectAsync(
        EndPoint endPoint,
        CancellationToken cancellationToken
    )
    {
        return _connectDelegate(endPoint);
    }
}
