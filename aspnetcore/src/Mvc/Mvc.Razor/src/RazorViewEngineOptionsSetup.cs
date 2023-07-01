using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Razor;

internal sealed class RazorViewEngineOptionsSetup : IConfigureOptions<RazorViewEngineOptions>
{
    public void Configure(RazorViewEngineOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
        options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);

        options.AreaViewLocationFormats.Add(
            "/Areas/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension
        );
        options.AreaViewLocationFormats.Add(
            "/Areas/{2}/Views/Shared/{0}" + RazorViewEngine.ViewExtension
        );
        options.AreaViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
    }
}
