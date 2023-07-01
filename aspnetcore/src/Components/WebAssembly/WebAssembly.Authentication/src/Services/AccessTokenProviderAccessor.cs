using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

internal sealed class AccessTokenProviderAccessor : IAccessTokenProviderAccessor
{
    private readonly IServiceProvider _provider;
    private IAccessTokenProvider? _tokenProvider;

    public AccessTokenProviderAccessor(IServiceProvider provider) => _provider = provider;

    public IAccessTokenProvider TokenProvider =>
        _tokenProvider ??= _provider.GetRequiredService<IAccessTokenProvider>();
}
