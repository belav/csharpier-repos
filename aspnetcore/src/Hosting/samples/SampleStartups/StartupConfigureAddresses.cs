// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

// Note that this sample will not run. It is only here to illustrate usage patterns.

namespace SampleStartups;

public class StartupConfigureAddresses : StartupBase
{
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public override void Configure(IApplicationBuilder app)
    {
        app.Run(
            async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            }
        );
    }

    // Entry point for the application.
    public static Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddCommandLine(args).Build();

        var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseConfiguration(config)
                    .UseKestrel()
                    .UseStartup<StartupConfigureAddresses>()
                    .UseUrls("http://localhost:5000", "http://localhost:5001");
            })
            .Build();

        return host.RunAsync();
    }
}
