using Microsoft.AspNetCore.Mvc;

namespace CorsWebSite;

public class StartupWithoutEndpointRouting : Startup
{
    public override void Configure(IApplicationBuilder app)
    {
        app.UseMvc();
    }

    protected override void ConfigureMvcOptions(MvcOptions options)
    {
        options.EnableEndpointRouting = false;
    }
}
