using System;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Experimental;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Experimental.Quic;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Quic <see cref="IWebHostBuilder"/> extensions.
    /// </summary>
    public static class WebHostBuilderMsQuicExtensions
    {
        public static IWebHostBuilder UseQuic(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<
                    IMultiplexedConnectionListenerFactory,
                    QuicTransportFactory
                >();
            });
        }

        public static IWebHostBuilder UseQuic(
            this IWebHostBuilder hostBuilder,
            Action<QuicTransportOptions> configureOptions
        )
        {
            return hostBuilder
                .UseQuic()
                .ConfigureServices(services =>
                {
                    services.Configure(configureOptions);
                });
        }
    }
}
