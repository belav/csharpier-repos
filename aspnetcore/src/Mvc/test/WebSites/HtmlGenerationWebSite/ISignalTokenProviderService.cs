using Microsoft.Extensions.Primitives;

namespace HtmlGenerationWebSite;

public interface ISignalTokenProviderService<TKey>
{
    IChangeToken GetToken(object key);

    void SignalToken(object key);
}
