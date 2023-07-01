using System;
using System.Reflection;
using Grpc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace InteropTestsWebsite;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
    {
        // Required to notify test infrastructure that it can begin tests
        applicationLifetime.ApplicationStarted.Register(() =>
        {
            Console.WriteLine("Application started.");

            var runtimeVersion =
                typeof(object).Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion ?? "Unknown";
            Console.WriteLine($"NetCoreAppVersion: {runtimeVersion}");
            var aspNetCoreVersion =
                typeof(HeaderNames).Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion ?? "Unknown";
            Console.WriteLine($"AspNetCoreAppVersion: {aspNetCoreVersion}");
        });

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<TestServiceImpl>();
        });
    }
}
