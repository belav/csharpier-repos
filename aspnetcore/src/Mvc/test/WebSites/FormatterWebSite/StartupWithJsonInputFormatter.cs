using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FormatterWebSite;

public class StartupWithJsonFormatter
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddMvc(options =>
            {
                options.ModelMetadataDetailsProviders.Add(
                    new SuppressChildValidationMetadataProvider(typeof(Developer))
                );
                options.ModelMetadataDetailsProviders.Add(
                    new SuppressChildValidationMetadataProvider(typeof(Supplier))
                );
            })
            .AddXmlDataContractSerializerFormatters();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
