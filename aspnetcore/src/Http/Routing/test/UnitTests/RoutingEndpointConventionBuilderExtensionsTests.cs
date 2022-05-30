// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Routing;

public class RoutingEndpointConventionBuilderExtensionsTests
{
    [Fact]
    public void RequireHost_HostNames()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        builder.RequireHost("contoso.com:8080");

        // Assert
        var convention = Assert.Single(builder.Conventions);

        var endpointModel = new RouteEndpointBuilder((context) => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0);
        convention(endpointModel);

        var hostMetadata = Assert.IsType<HostAttribute>(Assert.Single(endpointModel.Metadata));

        Assert.Equal("contoso.com:8080", hostMetadata.Hosts.Single());
    }

    private class TestEndpointConventionBuilder : IEndpointConventionBuilder
    {
        public IList<Action<EndpointBuilder>> Conventions { get; } = new List<Action<EndpointBuilder>>();

        public void Add(Action<EndpointBuilder> convention)
        {
            Conventions.Add(convention);
        }
    }
}
