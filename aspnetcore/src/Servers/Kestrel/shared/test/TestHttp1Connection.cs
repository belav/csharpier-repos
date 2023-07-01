using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Microsoft.AspNetCore.Testing;

internal class TestHttp1Connection : Http1Connection
{
    public TestHttp1Connection(HttpConnectionContext context)
        : base(context) { }

    public HttpVersion HttpVersionEnum
    {
        get => _httpVersion;
        set => _httpVersion = value;
    }

    public bool KeepAlive
    {
        get => _keepAlive;
        set => _keepAlive = value;
    }

    public MessageBody NextMessageBody { private get; set; }

    public Task ProduceEndAsync()
    {
        return ProduceEnd();
    }

    protected override MessageBody CreateMessageBody()
    {
        return NextMessageBody ?? base.CreateMessageBody();
    }
}
