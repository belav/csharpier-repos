using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.CompilationFeatureDetectorTest
{
    public class StartupWithMapBlazorHub
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
            });
        }

        public class App : Microsoft.AspNetCore.Components.ComponentBase { }
    }
}
