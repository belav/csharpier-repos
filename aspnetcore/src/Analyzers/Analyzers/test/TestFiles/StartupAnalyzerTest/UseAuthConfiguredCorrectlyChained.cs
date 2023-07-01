using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class UseAuthConfiguredCorrectlyChained
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting().UseAuthorization().UseEndpoints(r => { });
        }
    }
}
