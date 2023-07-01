using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.OutputCaching;

internal sealed class OutputCacheOptionsSetup : IConfigureOptions<OutputCacheOptions>
{
    private readonly IServiceProvider _services;

    public OutputCacheOptionsSetup(IServiceProvider services)
    {
        _services = services;
    }

    public void Configure(OutputCacheOptions options)
    {
        options.ApplicationServices = _services;
    }
}
