// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Discovery;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Components.Endpoints.Infrastructure;
using Microsoft.AspNetCore.Routing;
using static Microsoft.AspNetCore.Internal.LinkerFlags;

namespace Microsoft.AspNetCore.Components.Infrastructure;

internal class RazorComponentEndpointDataSourceFactory
{
    private readonly RazorComponentEndpointFactory _factory;
    private readonly IEnumerable<RenderModeEndpointProvider> _providers;
    private readonly HotReloadService? _hotReloadService;

    public RazorComponentEndpointDataSourceFactory(
        RazorComponentEndpointFactory factory,
        IEnumerable<RenderModeEndpointProvider> providers,
        HotReloadService? hotReloadService = null
    )
    {
        _factory = factory;
        _providers = providers;
        _hotReloadService = hotReloadService;
    }

    public RazorComponentEndpointDataSource<TRootComponent> CreateDataSource<
        [DynamicallyAccessedMembers(Component)] TRootComponent
    >(IEndpointRouteBuilder endpoints)
    {
        var builder =
            ComponentApplicationBuilder.GetBuilder<TRootComponent>()
            ?? DefaultRazorComponentApplication<TRootComponent>.Instance.GetBuilder();

        return new RazorComponentEndpointDataSource<TRootComponent>(
            builder,
            _providers,
            endpoints.CreateApplicationBuilder(),
            _factory,
            _hotReloadService
        );
    }
}
