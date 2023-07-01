using Microsoft.AspNetCore.Authentication.Cookies;

namespace RazorPagesWebSite;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => options.LoginPath = "/Login");

        services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/Admin");
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
        });
    }
}
