using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

internal sealed class RazorRuntimeCompilationHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        // Add Razor services
        builder.ConfigureServices(RazorRuntimeCompilationMvcCoreBuilderExtensions.AddServices);
    }
}
