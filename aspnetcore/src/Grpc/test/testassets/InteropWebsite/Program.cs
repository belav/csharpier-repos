using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InteropTestsWebsite;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                builder.AddConsole(loggerOptions =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    loggerOptions.DisableColors = true;
#pragma warning restore CS0618 // Type or member is obsolete
                });
                builder.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(
                    (context, options) =>
                    {
                        // Support --port and --use_tls cmdline arguments normally supported
                        // by gRPC interop servers.
                        var useTls = context.Configuration.GetValue("use_tls", false);

                        options.Limits.MinRequestBodyDataRate = null;
                        options.ListenAnyIP(
                            0,
                            listenOptions =>
                            {
                                Console.WriteLine($"Enabling connection encryption: {useTls}");

                                if (useTls)
                                {
                                    var basePath = Path.GetDirectoryName(
                                        typeof(Program).Assembly.Location
                                    );
                                    var certPath = Path.Combine(basePath!, "Certs", "server1.pfx");

                                    listenOptions.UseHttps(certPath, "1111");
                                }
                                listenOptions.Protocols = HttpProtocols.Http2;
                            }
                        );
                    }
                );
                webBuilder.UseStartup<Startup>();
            });
}
