using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Analyzers.TestFiles.StartupAnalyzerTest
{
    public class StartupSignatures_MoreVariety
    {
        public void ConfigureServices(IServiceCollection services) { }

        public void ConfigureServices(
            IServiceCollection services,
            StringBuilder s
        ) // Ignored
        { }

        public void Configure(
            StringBuilder s
        ) // Ignored,
        { }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) { }

        public void ConfigureProduction(IWebHostEnvironment env, IApplicationBuilder app) { }

        private void Configure(
            IApplicationBuilder app
        ) // Ignored
        { }
    }
}
