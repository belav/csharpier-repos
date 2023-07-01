using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class StartupSignatures_Standard
    {
        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app) { }
    }
}
