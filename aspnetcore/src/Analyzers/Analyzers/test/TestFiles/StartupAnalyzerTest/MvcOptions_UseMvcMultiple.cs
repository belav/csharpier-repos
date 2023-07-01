using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class MvcOptions_UseMvcMultiple
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            /*MM1*/app.UseMvcWithDefaultRoute();

            app.UseStaticFiles();
            app.UseMiddleware<AuthorizationMiddleware>();

            /*MM2*/app.UseMvc();

            app.UseRouting();
            app.UseEndpoints(endpoints => { });

            /*MM3*/app.UseMvc();
        }
    }
}
