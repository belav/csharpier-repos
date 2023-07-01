using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Internal;

namespace Microsoft.AspNetCore.Hosting
{
    internal interface ISupportsStartup
    {
        IWebHostBuilder Configure(Action<WebHostBuilderContext, IApplicationBuilder> configure);
        IWebHostBuilder UseStartup(
            [DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType
        );
        IWebHostBuilder UseStartup<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TStartup
        >(Func<WebHostBuilderContext, TStartup> startupFactory);
    }
}
