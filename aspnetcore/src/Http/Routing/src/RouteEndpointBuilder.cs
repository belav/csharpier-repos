// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Routing;

/// <summary>
/// Supports building a new <see cref="RouteEndpoint"/>.
/// </summary>
public sealed class RouteEndpointBuilder : EndpointBuilder
{
    /// <summary>
    /// Gets or sets the <see cref="RoutePattern"/> associated with this endpoint.
    /// </summary>
    public RoutePattern RoutePattern { get; set; }

    /// <summary>
    ///  Gets or sets the order assigned to the endpoint.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Constructs a new <see cref="RouteEndpointBuilder"/> instance.
    /// </summary>
    /// <param name="requestDelegate">The delegate used to process requests for the endpoint.</param>
    /// <param name="routePattern">The <see cref="RoutePattern"/> to use in URL matching.</param>
    /// <param name="order">The order assigned to the endpoint.</param>
    public RouteEndpointBuilder(
       RequestDelegate requestDelegate,
       RoutePattern routePattern,
       int order)
    {
        RequestDelegate = requestDelegate;
        RoutePattern = routePattern;
        Order = order;
    }

    /// <summary>
    /// Constructs a new <see cref="RouteEndpointBuilder"/> instance.
    /// </summary>
    /// <param name="routePattern">The <see cref="RoutePattern"/> to use in URL matching.</param>
    /// <param name="order">The order assigned to the endpoint.</param>
    /// <remarks>
    /// This constructor allows the <see cref="RequestDelegate"/> to be added to the
    /// <see cref="RouteEndpointBuilder"/> after construction but before <see cref="Build"/>
    /// is invoked.
    /// </remarks>
    internal RouteEndpointBuilder(
        RoutePattern routePattern,
        int order)
    {
        RoutePattern = routePattern;
        Order = order;
    }

    /// <inheritdoc />
    public override Endpoint Build()
    {
        if (RequestDelegate is null)
        {
            throw new InvalidOperationException($"{nameof(RequestDelegate)} must be specified to construct a {nameof(RouteEndpoint)}.");
        }

        var routeEndpoint = new RouteEndpoint(
            RequestDelegate,
            RoutePattern,
            Order,
            new EndpointMetadataCollection(Metadata),
            DisplayName);

        return routeEndpoint;
    }
}
