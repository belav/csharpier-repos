using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class UseAuthBeforeUseRouting
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseFileServer();
            /*MM*/app.UseAuthorization();
            app.UseRouting();
            app.UseEndpoints(r => { });
        }
    }
}
