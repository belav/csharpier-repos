// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestServer
{
    // Used for E2E tests that verify different overloads of MapFallbackToClientSideBlazor.
    public class StartupWithMapFallbackToClientSideBlazor
    {
        public StartupWithMapFallbackToClientSideBlazor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var enUs = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUs;
            CultureInfo.DefaultThreadCurrentUICulture = enUs;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // The client-side files middleware needs to be here because the base href in hardcoded to /subdir/
            app.Map("/subdir", subApp =>
            {
                subApp.UseBlazorFrameworkFiles();
                subApp.UseStaticFiles();

                // The calls to `Map` allow us to test each of these overloads, while keeping them isolated.
                subApp.Map("/filepath", filepath =>
                {
                    filepath.UseRouting();
                    filepath.UseEndpoints(endpoints =>
                    {
                        endpoints.MapFallbackToFile("index.html");
                    });
                });
                subApp.Map("/pattern_filepath", patternFilePath =>
                {
                    patternFilePath.UseRouting();
                    patternFilePath.UseEndpoints(endpoints =>
                    {
                        endpoints.MapFallbackToFile("test/{*path:nonfile}", "index.html");
                    });
                });
                subApp.Map("/assemblypath_filepath", assemblyPathFilePath =>
                {
                    assemblyPathFilePath.UseRouting();
                    assemblyPathFilePath.UseEndpoints(endpoints =>
                    {
                        endpoints.MapFallbackToFile("index.html");
                    });
                });
                subApp.Map("/assemblypath_pattern_filepath", assemblyPatternFilePath =>
                {
                    assemblyPatternFilePath.UseRouting();
                    assemblyPatternFilePath.UseEndpoints(endpoints =>
                    {
                        endpoints.MapFallbackToFile("test/{*path:nonfile}", "index.html");
                    });
                });
            });
        }
    }
}
