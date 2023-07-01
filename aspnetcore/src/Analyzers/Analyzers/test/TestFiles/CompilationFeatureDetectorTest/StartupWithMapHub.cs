using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.CompilationFeatureDetectorTest
{
    public class StartupWithMapHub
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MyHub>("/test");
            });
        }
    }

    public class MyHub : Hub { }
}
