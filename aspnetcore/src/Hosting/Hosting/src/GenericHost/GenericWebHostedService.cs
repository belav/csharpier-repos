// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Hosting
{
    internal class GenericWebHostService : IHostedService
    {
        public GenericWebHostService(IOptions<GenericWebHostServiceOptions> options,
                                     IServer server,
                                     ILoggerFactory loggerFactory,
                                     DiagnosticListener diagnosticListener,
                                     ActivitySource activitySource,
                                     IHttpContextFactory httpContextFactory,
                                     IApplicationBuilderFactory applicationBuilderFactory,
                                     IEnumerable<IStartupFilter> startupFilters,
                                     IConfiguration configuration,
                                     IWebHostEnvironment hostingEnvironment)
        {
            Options = options.Value;
            Server = server;
            Logger = loggerFactory.CreateLogger("Microsoft.AspNetCore.Hosting.Diagnostics");
            LifetimeLogger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
            DiagnosticListener = diagnosticListener;
            ActivitySource = activitySource;
            HttpContextFactory = httpContextFactory;
            ApplicationBuilderFactory = applicationBuilderFactory;
            StartupFilters = startupFilters;
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public GenericWebHostServiceOptions Options { get; }
        public IServer Server { get; }
        public ILogger Logger { get; }
        // Only for high level lifetime events
        public ILogger LifetimeLogger { get; }
        public DiagnosticListener DiagnosticListener { get; }
        public ActivitySource ActivitySource { get; }
        public IHttpContextFactory HttpContextFactory { get; }
        public IApplicationBuilderFactory ApplicationBuilderFactory { get; }
        public IEnumerable<IStartupFilter> StartupFilters { get; }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            HostingEventSource.Log.HostStart();

            var serverAddressesFeature = Server.Features.Get<IServerAddressesFeature>();
            var addresses = serverAddressesFeature?.Addresses;
            if (addresses != null && !addresses.IsReadOnly && addresses.Count == 0)
            {
                var urls = Configuration[WebHostDefaults.ServerUrlsKey];
                if (!string.IsNullOrEmpty(urls))
                {
                    serverAddressesFeature!.PreferHostingUrls = WebHostUtilities.ParseBool(Configuration, WebHostDefaults.PreferHostingUrlsKey);

                    foreach (var value in urls.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        addresses.Add(value);
                    }
                }
            }

            RequestDelegate? application = null;

            try
            {
                var configure = Options.ConfigureApplication;

                if (configure == null)
                {
                    throw new InvalidOperationException($"No application configured. Please specify an application via IWebHostBuilder.UseStartup, IWebHostBuilder.Configure, or specifying the startup assembly via {nameof(WebHostDefaults.StartupAssemblyKey)} in the web host configuration.");
                }

                var builder = ApplicationBuilderFactory.CreateBuilder(Server.Features);

                foreach (var filter in StartupFilters.Reverse())
                {
                    configure = filter.Configure(configure);
                }

                configure(builder);

                // Build the request pipeline
                application = builder.Build();
            }
            catch (Exception ex)
            {
                Logger.ApplicationError(ex);

                if (!Options.WebHostOptions.CaptureStartupErrors)
                {
                    throw;
                }

                var showDetailedErrors = HostingEnvironment.IsDevelopment() || Options.WebHostOptions.DetailedErrors;

                application = ErrorPageBuilder.BuildErrorPageApplication(HostingEnvironment.ContentRootFileProvider, Logger, showDetailedErrors, ex);
            }

            var httpApplication = new HostingApplication(application, Logger, DiagnosticListener, ActivitySource, HttpContextFactory);

            await Server.StartAsync(httpApplication, cancellationToken);

            if (addresses != null)
            {
                foreach (var address in addresses)
                {
                    LifetimeLogger.ListeningOnAddress(address);
                }
            }

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var assembly in Options.WebHostOptions.GetFinalHostingStartupAssemblies())
                {
                    Logger.StartupAssemblyLoaded(assembly);
                }
            }

            if (Options.HostingStartupExceptions != null)
            {
                foreach (var exception in Options.HostingStartupExceptions.InnerExceptions)
                {
                    Logger.HostingStartupAssemblyError(exception);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Server.StopAsync(cancellationToken);
            }
            finally
            {
                HostingEventSource.Log.HostStop();
            }
        }
    }
}
