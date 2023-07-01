using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace RoutingWebSite;

public class StartupForGroups
{
    // Set up application services
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().AddNewtonsoftJson();

        // Used by some controllers defined in this project.
        services.Configure<RouteOptions>(
            options => options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer)
        );
        services.AddScoped<TestResponseGenerator>();
        // This is used by test response generator
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
    }

    public virtual void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            var pagesGroup = endpoints.MapGroup("/pages");
            pagesGroup.MapRazorPages();

            var controllerGroup = endpoints.MapGroup("/controllers/{org}");
            controllerGroup.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action}/{id?}"
            );
        });
    }
}
