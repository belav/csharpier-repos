// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using RoutingWebSite;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class RoutingAcrossPipelineBranchesTests : IClassFixture<MvcTestFixture<RoutingWebSite.StartupRoutingDifferentBranches>>
    {
        public RoutingAcrossPipelineBranchesTests(MvcTestFixture<RoutingWebSite.StartupRoutingDifferentBranches> fixture)
        {
            Factory = fixture.Factories.FirstOrDefault() ?? fixture.WithWebHostBuilder(ConfigureWebHostBuilder);
        }

        private static void ConfigureWebHostBuilder(IWebHostBuilder builder) => builder.UseStartup<RoutingWebSite.StartupRoutingDifferentBranches>();

        public WebApplicationFactory<StartupRoutingDifferentBranches> Factory { get; }

        [Fact]
        public async Task MatchesConventionalRoutesInTheirBranches()
        {
            var client = Factory.CreateClient();

            // Arrange
            var subdirRequest = new HttpRequestMessage(HttpMethod.Get, "subdir/literal/Branches/Index/s");
            var commonRequest = new HttpRequestMessage(HttpMethod.Get, "common/Branches/Index/c/literal");
            var defaultRequest = new HttpRequestMessage(HttpMethod.Get, "Branches/literal/Index/d");

            // Act
            var subdirResponse = await client.SendAsync(subdirRequest);
            var subdirContent = await subdirResponse.Content.ReadFromJsonAsync<RouteInfo>();

            var commonResponse = await client.SendAsync(commonRequest);
            var commonContent = await commonResponse.Content.ReadFromJsonAsync<RouteInfo>();

            var defaultResponse = await client.SendAsync(defaultRequest);
            var defaultContent = await defaultResponse.Content.ReadFromJsonAsync<RouteInfo>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, subdirResponse.StatusCode);
            Assert.True(subdirContent.RouteValues.TryGetValue("subdir", out var subdir));
            Assert.Equal("s", subdir);

            Assert.Equal(HttpStatusCode.OK, commonResponse.StatusCode);
            Assert.True(commonContent.RouteValues.TryGetValue("common", out var common));
            Assert.Equal("c", common);

            Assert.Equal(HttpStatusCode.OK, defaultResponse.StatusCode);
            Assert.True(defaultContent.RouteValues.TryGetValue("default", out var @default));
            Assert.Equal("d", @default);
        }

        [Fact]
        public async Task LinkGenerationWorksOnEachBranch()
        {
            var client = Factory.CreateClient();
            var linkQuery = "?link";

                // Arrange
            var subdirRequest = new HttpRequestMessage(HttpMethod.Get, "subdir/literal/Branches/Index/s" + linkQuery);
            var commonRequest = new HttpRequestMessage(HttpMethod.Get, "common/Branches/Index/c/literal" + linkQuery);
            var defaultRequest = new HttpRequestMessage(HttpMethod.Get, "Branches/literal/Index/d" + linkQuery);

            // Act
            var subdirResponse = await client.SendAsync(subdirRequest);
            var subdirContent = await subdirResponse.Content.ReadFromJsonAsync<RouteInfo>();

            var commonResponse = await client.SendAsync(commonRequest);
            var commonContent = await commonResponse.Content.ReadFromJsonAsync<RouteInfo>();

            var defaultResponse = await client.SendAsync(defaultRequest);
            var defaultContent = await defaultResponse.Content.ReadFromJsonAsync<RouteInfo>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, subdirResponse.StatusCode);
            Assert.Equal("/subdir/literal/Branches/Index/s", subdirContent.Link);

            Assert.Equal(HttpStatusCode.OK, commonResponse.StatusCode);
            Assert.Equal("/common/Branches/Index/c/literal", commonContent.Link);

            Assert.Equal(HttpStatusCode.OK, defaultResponse.StatusCode);
            Assert.Equal("/Branches/literal/Index/d", defaultContent.Link);
        }

        // This still works because even though each middleware now gets its own data source,
        // those data sources still get added to a global collection in IOptions<RouteOptions>>.DataSources
        [Fact]
        public async Task LinkGenerationStillWorksAcrossBranches()
        {
            var client = Factory.CreateClient();
            var linkQuery = "?link";

            // Arrange
            var subdirRequest = new HttpRequestMessage(HttpMethod.Get, "subdir/literal/Branches/Index/s" + linkQuery + "&link_common=c&link_subdir");
            var defaultRequest = new HttpRequestMessage(HttpMethod.Get, "Branches/literal/Index/d" + linkQuery + "&link_subdir=s");

            // Act
            var subdirResponse = await client.SendAsync(subdirRequest);
            var subdirContent = await subdirResponse.Content.ReadFromJsonAsync<RouteInfo>();

            var defaultResponse = await client.SendAsync(defaultRequest);
            var defaultContent = await defaultResponse.Content.ReadFromJsonAsync<RouteInfo>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, subdirResponse.StatusCode);
            // Note that this link and the one below don't account for the path base being in a different branch.
            // The recommendation for customers doing link generation across branches will be to always generate absolute
            // URIs by explicitly passing the path base to the link generator.
            // In the future there are improvements we might be able to do in most common cases to lift this limitation if we receive
            // feedback about it.
            Assert.Equal("/subdir/Branches/Index/c/literal", subdirContent.Link);

            Assert.Equal(HttpStatusCode.OK, defaultResponse.StatusCode);
            Assert.Equal("/literal/Branches/Index/s", defaultContent.Link);
        }

        [Fact]
        public async Task DoesNotMatchConventionalRoutesDefinedInOtherBranches()
        {
            var client = Factory.CreateClient();

            // Arrange
            var commonRequest = new HttpRequestMessage(HttpMethod.Get, "common/literal/Branches/Index/s");
            var subdirRequest = new HttpRequestMessage(HttpMethod.Get, "subdir/Branches/Index/c/literal");
            var defaultRequest = new HttpRequestMessage(HttpMethod.Get, "common/Branches/literal/Index/d");

            // Act
            var commonResponse = await client.SendAsync(commonRequest);

            var subdirResponse = await client.SendAsync(subdirRequest);

            var defaultResponse = await client.SendAsync(defaultRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, commonResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, subdirResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, defaultResponse.StatusCode);
        }

        [Fact]
        public async Task ConventionalAndDynamicRouteOrdersAreScopedPerBranch()
        {
            var client = Factory.CreateClient();

            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "dynamicattributeorder/dynamic/route/rest");

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadFromJsonAsync<RouteInfo>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(content.RouteValues.TryGetValue("action", out var action));

            // The dynamic route wins because it has Order 1 (scope to that router) and
            // has higher precedence.
            Assert.Equal("Index", action);
        }

        private record RouteInfo(string RouteName, IDictionary<string, string> RouteValues, string Link);
    }
}
