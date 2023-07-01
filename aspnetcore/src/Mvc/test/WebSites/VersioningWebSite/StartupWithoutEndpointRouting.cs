using Microsoft.AspNetCore.Mvc;

namespace VersioningWebSite;

public class StartupWithoutEndpointRouting : Startup
{
    public override void Configure(IApplicationBuilder app)
    {
        app.UseMvcWithDefaultRoute();
    }

    protected override void ConfigureMvcOptions(MvcOptions options)
    {
        options.EnableEndpointRouting = false;
    }
}
