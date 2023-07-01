using BasicWebSite.ValueProviders;

namespace BasicWebSite;

public class StartupWithCustomValueProvider
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(o =>
        {
            o.ValueProviderFactories.Add(new CustomValueProviderFactory());
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints((endpoints) => endpoints.MapDefaultControllerRoute());
    }
}
