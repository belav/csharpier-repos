using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace Microsoft.AspNetCore.Http.Connections.Client.Internal;

internal interface ITransport : IDuplexPipe
{
    Task StartAsync(
        Uri url,
        TransferFormat transferFormat,
        CancellationToken cancellationToken = default
    );
    Task StopAsync();
}
