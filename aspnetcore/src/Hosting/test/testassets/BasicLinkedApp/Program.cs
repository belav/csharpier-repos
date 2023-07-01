using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BasicLinkedApp;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    // Do not change the signature of this method. It's used for tests.
    private static IHostBuilder CreateWebHostBuilder(string[] args)
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(config =>
            {
                config.AddCommandLine(args);
            })
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseKestrel(o =>
                    {
                        o.ConfigureEndpointDefaults(lo =>
                        {
                            lo.UseConnectionLogging();
                        });
                    })
                    .UseStartup<Startup>();
            });
    }
}
