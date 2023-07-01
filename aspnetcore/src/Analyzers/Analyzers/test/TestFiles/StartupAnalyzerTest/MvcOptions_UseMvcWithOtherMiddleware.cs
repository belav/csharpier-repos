using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class MvcOptions_UseMvcWithOtherMiddleware
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseMiddleware<AuthorizationMiddleware>();

            /*MM*/app.UseMvc();

            app.UseRouting();
            app.UseEndpoints(endpoints => { });
        }
    }
}
