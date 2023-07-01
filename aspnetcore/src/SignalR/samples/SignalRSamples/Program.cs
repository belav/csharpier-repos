using System.Net;
using Microsoft.AspNetCore.SignalR;
using SignalRSamples.Hubs;

namespace SignalRSamples;

public class Program
{
    public static Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddCommandLine(args).Build();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseConfiguration(config)
                    .UseSetting(WebHostDefaults.PreventHostingStartupKey, "true")
                    .ConfigureLogging(
                        (c, factory) =>
                        {
                            factory.AddConfiguration(c.Configuration.GetSection("Logging"));
                            factory.AddConsole();
                            //factory.SetMinimumLevel(LogLevel.Trace);
                        }
                    )
                    .UseKestrel(options =>
                    {
                        // Default port
                        options.ListenAnyIP(0);

                        // Hub bound to TCP end point
                        //options.Listen(IPAddress.Any, 9001, builder =>
                        //{
                        //    builder.UseHub<Chat>();
                        //});
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();
            })
            .Build();

        return host.RunAsync();
    }
}
