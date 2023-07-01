using System;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Testing;

public class LifetimeNotImplemented : IHostApplicationLifetime
{
    public CancellationToken ApplicationStarted
    {
        get { throw new NotImplementedException(); }
    }

    public CancellationToken ApplicationStopped
    {
        get { throw new NotImplementedException(); }
    }

    public CancellationToken ApplicationStopping
    {
        get { throw new NotImplementedException(); }
    }

    public void StopApplication()
    {
        throw new NotImplementedException();
    }
}
