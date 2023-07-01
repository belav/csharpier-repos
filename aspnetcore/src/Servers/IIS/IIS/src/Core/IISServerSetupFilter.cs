using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Server.IIS.Core;

internal sealed class IISServerSetupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            var server = app.ApplicationServices.GetService<IServer>();
            if (server?.GetType() != typeof(IISHttpServer))
            {
                throw new InvalidOperationException(
                    "Application is running inside IIS process but is not configured to use IIS server."
                );
            }

            next(app);
        };
    }
}
