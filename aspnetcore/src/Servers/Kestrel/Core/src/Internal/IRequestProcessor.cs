using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting.Server;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal;

internal interface IRequestProcessor
{
    Task ProcessRequestsAsync<TContext>(IHttpApplication<TContext> application)
        where TContext : notnull;
    void StopProcessingNextRequest();
    void HandleRequestHeadersTimeout();
    void HandleReadDataRateTimeout();
    void OnInputOrOutputCompleted();
    void Tick(DateTimeOffset now);
    void Abort(ConnectionAbortedException ex);
}
