using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;using static Microsoft.AspNetCore.Internal.LinkerFlags;

namespace Microsoft.AspNetCore.Components.WebAssembly.Authentication;

internal sealed class DefaultRemoteApplicationPathsProvider<
    [DynamicallyAccessedMembers(JsonSerialized)] TProviderOptions
> : IRemoteAuthenticationPathsProvider
    where TProviderOptions : class, new()
{
    private readonly IOptions<RemoteAuthenticationOptions<TProviderOptions>> _options;

    public DefaultRemoteApplicationPathsProvider(
        IOptionsSnapshot<RemoteAuthenticationOptions<TProviderOptions>> options
    )
    {
        _options = options;
    }

    public RemoteAuthenticationApplicationPathsOptions ApplicationPaths =>
        _options.Value.AuthenticationPaths;
}
