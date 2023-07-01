using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class UseAuthBeforeUseRoutingChained
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseFileServer().UseAuthorization().UseRouting().UseEndpoints(r => { });
        }
    }
}
