// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Experimental;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.InMemory.FunctionalTests.TestTransport
{
    /// <summary>
    /// In-memory TestServer
    /// </summary
    internal class TestServer : IAsyncDisposable, IStartup
    {
        private readonly MemoryPool<byte> _memoryPool;
        private readonly RequestDelegate _app;
        private readonly InMemoryTransportFactory _transportFactory;
        private readonly IHost _host;

        public TestServer(RequestDelegate app)
            : this(app, new TestServiceContext())
        {
        }

        public TestServer(RequestDelegate app, TestServiceContext context)
            : this(app, context, new ListenOptions(new IPEndPoint(IPAddress.Loopback, 0)))
        {
            // The endpoint is ignored, but this ensures no cert loading happens for HTTPS endpoints.
        }

        public TestServer(RequestDelegate app, TestServiceContext context, ListenOptions listenOptions)
            : this(app, context, options => options.CodeBackedListenOptions.Add(listenOptions), _ => { })
        {
        }

        public TestServer(RequestDelegate app, TestServiceContext context, Action<ListenOptions> configureListenOptions)
            : this(app, context, options =>
                {
                    var listenOptions = new ListenOptions(new IPEndPoint(IPAddress.Loopback, 0))
                    {
                        KestrelServerOptions = options
                    };

                    configureListenOptions(listenOptions);
                    options.CodeBackedListenOptions.Add(listenOptions);
                },
                _ => { })
        {
        }

        public TestServer(RequestDelegate app, TestServiceContext context, Action<KestrelServerOptions> configureKestrel, Action<IServiceCollection> configureServices)
        {
            _app = app;
            Context = context;
            _memoryPool = context.MemoryPoolFactory();
            _transportFactory = new InMemoryTransportFactory();
            HttpClientSlim = new InMemoryHttpClientSlim(this);

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseSetting(WebHostDefaults.ShutdownTimeoutKey, TestConstants.DefaultTimeout.TotalSeconds.ToString(CultureInfo.InvariantCulture))
                        .Configure(app => { app.Run(_app); });
                })
                .ConfigureServices(services =>
                {
                    configureServices(services);

                    // Ensure there is at least one multiplexed connection lister factory if none was added to services.
                    if (!services.Any(d => d.ServiceType == typeof(IMultiplexedConnectionListenerFactory)))
                    {
                        // Mock multiplexed connection listner is added so Kestrel doesn't error
                        // when a HTTP/3 endpoint is configured.
                        services.AddSingleton<IMultiplexedConnectionListenerFactory>(new MockMultiplexedConnectionListenerFactory());
                    }

                    services.AddSingleton<IStartup>(this);
                    services.AddSingleton(context.LoggerFactory);

                    services.AddSingleton<IServer>(sp =>
                    {
                        context.ServerOptions.ApplicationServices = sp;
                        configureKestrel(context.ServerOptions);
                        return new KestrelServerImpl(
                            new IConnectionListenerFactory[] { _transportFactory },
                            sp.GetServices<IMultiplexedConnectionListenerFactory>(),
                            context);
                    });
                });

            _host = hostBuilder.Build();
            _host.Start();
        }

        public int Port => 0;

        public TestServiceContext Context { get; }

        public InMemoryHttpClientSlim HttpClientSlim { get; }

        public InMemoryConnection CreateConnection()
        {
            var transportConnection = new InMemoryTransportConnection(_memoryPool, Context.Log, Context.Scheduler);
            _transportFactory.AddConnection(transportConnection);
            return new InMemoryConnection(transportConnection);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return _host.StopAsync(cancellationToken);
        }


        void IStartup.Configure(IApplicationBuilder app)
        {
            app.Run(_app);
        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }

        public async ValueTask DisposeAsync()
        {
            await _host.StopAsync().ConfigureAwait(false);
            // The concrete Host implements IAsyncDisposable
            await ((IAsyncDisposable)_host).DisposeAsync().ConfigureAwait(false);
            _memoryPool.Dispose();
        }
    }
}
