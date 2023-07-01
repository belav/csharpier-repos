using Microsoft.AspNetCore.Hosting.Server.Abstractions;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

internal sealed class Http1Connection<TContext> : Http1Connection, IHostContextContainer<TContext>
    where TContext : notnull
{
    public Http1Connection(HttpConnectionContext context)
        : base(context) { }

    TContext? IHostContextContainer<TContext>.HostContext { get; set; }
}
