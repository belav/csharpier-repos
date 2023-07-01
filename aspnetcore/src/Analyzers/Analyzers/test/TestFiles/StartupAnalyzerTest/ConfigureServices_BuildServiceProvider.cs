using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class ConfigureServices_BuildServiceProvider
    {
        public void ConfigureServices(IServiceCollection services)
        {
            /*MM1*/services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app) { }
    }
}
