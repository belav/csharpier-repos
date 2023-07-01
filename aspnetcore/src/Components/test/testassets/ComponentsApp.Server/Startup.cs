using Microsoft.AspNetCore.Components.Server.Circuits;

namespace ComponentsApp.Server;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        services.AddSingleton<CircuitHandler, LoggingCircuitHandler>();
        services.AddServerSideBlazor(options =>
        {
            options.DetailedErrors = true;
        });

        services.AddSingleton<WeatherForecastService, DefaultWeatherForecastService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
