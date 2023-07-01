using System;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http2Cat;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;

namespace Microsoft.Extensions.DependencyInjection;

internal static class Http2CatIServiceCollectionExtensions
{
    public static IServiceCollection UseHttp2Cat(
        this IServiceCollection services,
        Action<Http2CatOptions> configureOptions
    )
    {
        services.AddSingleton<IConnectionFactory, SocketConnectionFactory>();
        services.AddHostedService<Http2CatHostedService>();
        services.Configure(configureOptions);
        return services;
    }
}
