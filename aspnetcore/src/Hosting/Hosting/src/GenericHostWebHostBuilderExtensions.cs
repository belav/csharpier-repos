// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Contains extensions for an <see cref="IHostBuilder"/>.
    /// </summary>
    public static class GenericHostWebHostBuilderExtensions
    {
        /// <summary>
        /// Adds and configures an ASP.NET Core web application.
        /// </summary>
        public static IHostBuilder ConfigureWebHost(this IHostBuilder builder, Action<IWebHostBuilder> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            return builder.ConfigureWebHost(configure, _ => { });
        }

        /// <summary>
        /// Adds and configures an ASP.NET Core web application.
        /// </summary>
        public static IHostBuilder ConfigureWebHost(this IHostBuilder builder, Action<IWebHostBuilder> configure, Action<WebHostBuilderOptions> configureWebHostBuilder)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            if (configureWebHostBuilder is null)
            {
                throw new ArgumentNullException(nameof(configureWebHostBuilder));
            }

            var webHostBuilderOptions = new WebHostBuilderOptions();
            configureWebHostBuilder(webHostBuilderOptions);
            var webhostBuilder = new GenericWebHostBuilder(builder, webHostBuilderOptions);
            configure(webhostBuilder);
            builder.ConfigureServices((context, services) => services.AddHostedService<GenericWebHostService>());
            return builder;
        }
    }
}
