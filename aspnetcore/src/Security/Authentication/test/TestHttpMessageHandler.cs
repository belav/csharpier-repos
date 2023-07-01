using System.Net.Http;

namespace Microsoft.AspNetCore.Authentication;

public class TestHttpMessageHandler : HttpMessageHandler
{
    public Func<HttpRequestMessage, HttpResponseMessage> Sender { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        System.Threading.CancellationToken cancellationToken
    )
    {
        if (Sender != null)
        {
            return Task.FromResult(Sender(request));
        }

        return Task.FromResult<HttpResponseMessage>(null);
    }
}
