// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Extension methods for the IIS In-Process.
    /// </summary>
    public static class WebHostBuilderIISExtensions
    {
        /// <summary>
        /// Configures the port and base path the server should listen on when running behind AspNetCoreModule.
        /// The app will also be configured to capture startup errors.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseIIS(this IWebHostBuilder hostBuilder)
        {
            if (hostBuilder == null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }

            // Check if in process
            if (OperatingSystem.IsWindows() && NativeMethods.IsAspNetCoreModuleLoaded())
            {
                var iisConfigData = NativeMethods.HttpGetApplicationProperties();
                // Trim trailing slash to be consistent with other servers
                var contentRoot = iisConfigData.pwzFullApplicationPath.TrimEnd(Path.DirectorySeparatorChar);
                hostBuilder.UseContentRoot(contentRoot);
                return hostBuilder.ConfigureServices(
                    services =>
                    {
                        services.AddSingleton(new IISNativeApplication(new NativeSafeHandle(iisConfigData.pNativeApplication)));
                        services.AddSingleton<IServer, IISHttpServer>();
                        services.AddTransient<IISServerAuthenticationHandlerInternal>();
                        services.AddSingleton<IStartupFilter>(new IISServerSetupFilter(iisConfigData.pwzVirtualApplicationPath));
                        services.AddAuthenticationCore();
                        services.AddSingleton<IServerIntegratedAuth>(_ => new ServerIntegratedAuth()
                        {
                            IsEnabled = iisConfigData.fWindowsAuthEnabled || iisConfigData.fBasicAuthEnabled,
                            AuthenticationScheme = IISServerDefaults.AuthenticationScheme
                        });
                        services.Configure<IISServerOptions>(
                            options =>
                            {
                                options.ServerAddresses = iisConfigData.pwzBindings.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                options.ForwardWindowsAuthentication = iisConfigData.fWindowsAuthEnabled || iisConfigData.fBasicAuthEnabled;
                                options.MaxRequestBodySize = iisConfigData.maxRequestBodySize;
                                options.IisMaxRequestSizeLimit = iisConfigData.maxRequestBodySize;
                            }
                        );
                    });
            }

            return hostBuilder;
        }
    }
}
