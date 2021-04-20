// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JwtSample
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                    .ConfigureLogging(factory =>
                    {
                        factory.AddConsole();
                        factory.AddFilter("Console", level => level >= LogLevel.Information);
                        factory.AddDebug();
                    })
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                })
                .Build()
                .RunAsync();
        }
    }
}
