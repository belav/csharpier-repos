// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Routing.Matching
{
    public class DfaMatcherConformanceTest : FullFeaturedMatcherConformanceTest
    {
        // See the comments in the base class. DfaMatcher fixes a long-standing bug
        // with catchall parameters and empty segments.
        public override async Task Quirks_CatchAllParameter(string template, string path, string[] keys, string[] values)
        {
            // Arrange
            var (matcher, endpoint) = CreateMatcher(template);
            var httpContext = CreateContext(path);

            // Act
            await matcher.MatchAsync(httpContext);

            // Assert
            MatcherAssert.AssertMatch(httpContext, endpoint, keys, values);
        }

        // https://github.com/dotnet/aspnetcore/issues/18677
        [Theory]
        [InlineData("/middleware", 1)]
        [InlineData("/middleware/test", 1)]
        [InlineData("/middleware/test1/test2", 1)]
        [InlineData("/bill/boga", 0)]
        public async Task Match_Regression_1867_CorrectBehavior(string path, int endpointIndex)
        {
            var endpoints = new RouteEndpoint[]
            {
                EndpointFactory.CreateRouteEndpoint(
                    "{firstName}/{lastName}",
                    order: 0,
                    defaults: new { controller = "TestRoute", action = "Index", }),

                EndpointFactory.CreateRouteEndpoint(
                    "middleware/{**_}",
                    order: 0),
            };

            var expected = endpoints[endpointIndex];

            var matcher = CreateMatcher(useCorrectCatchAllBehavior: true, endpoints);
            var httpContext = CreateContext(path);

            // Act
            await matcher.MatchAsync(httpContext);

            // Assert
            MatcherAssert.AssertMatch(httpContext, expected, ignoreValues: true);
        }

        // https://github.com/dotnet/aspnetcore/issues/18677
        //
        [Theory]
        [InlineData("/middleware", 1)]
        [InlineData("/middleware/test", 1)]
        [InlineData("/middleware/test1/test2", 1)]
        [InlineData("/bill/boga", 0)]
        public async Task Match_Regression_1867_DefaultBehavior(string path, int endpointIndex)
        {
            var endpoints = new RouteEndpoint[]
            {
                EndpointFactory.CreateRouteEndpoint(
                    "{firstName}/{lastName}",
                    order: 0,
                    defaults: new { controller = "TestRoute", action = "Index", }),

                EndpointFactory.CreateRouteEndpoint(
                    "middleware/{**_}",
                    order: 0),
            };

            var expected = endpointIndex switch
            {
                -1 => null,
                _ => endpoints[endpointIndex],
            };

            var matcher = CreateMatcher(useCorrectCatchAllBehavior: default, endpoints);
            var httpContext = CreateContext(path);

            // Act
            await matcher.MatchAsync(httpContext);

            // Assert
            if (expected == null)
            {
                MatcherAssert.AssertNotMatch(httpContext);
            }
            else
            {
                MatcherAssert.AssertMatch(httpContext, expected, ignoreValues: true);
            }
        }

        // https://github.com/dotnet/aspnetcore/issues/18677
        //
        [Theory]
        [InlineData("/middleware", 1)]
        [InlineData("/middleware/test", 0)]
        [InlineData("/middleware/test1/test2", -1)]
        [InlineData("/bill/boga", 0)]
        public async Task Match_Regression_1867_LegacyBehavior(string path, int endpointIndex)
        {
            var endpoints = new RouteEndpoint[]
            {
                EndpointFactory.CreateRouteEndpoint(
                    "{firstName}/{lastName}",
                    order: 0,
                    defaults: new { controller = "TestRoute", action = "Index", }),

                EndpointFactory.CreateRouteEndpoint(
                    "middleware/{**_}",
                    order: 0),
            };

            var expected = endpointIndex switch
            {
                -1 => null,
                _ => endpoints[endpointIndex],
            };

            var matcher = CreateMatcher(useCorrectCatchAllBehavior: false, endpoints);
            var httpContext = CreateContext(path);

            // Act
            await matcher.MatchAsync(httpContext);

            // Assert
            if (expected == null)
            {
                MatcherAssert.AssertNotMatch(httpContext);
            }
            else
            {
                MatcherAssert.AssertMatch(httpContext, expected, ignoreValues: true);
            }
        }

        internal override Matcher CreateMatcher(params RouteEndpoint[] endpoints)
        {
            return CreateMatcher(useCorrectCatchAllBehavior: default, endpoints);
        }

        internal Matcher CreateMatcher(bool? useCorrectCatchAllBehavior, params RouteEndpoint[] endpoints)
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .AddRouting()
                .BuildServiceProvider();

            var builder = services.GetRequiredService<DfaMatcherBuilder>();
            if (useCorrectCatchAllBehavior.HasValue)
            {
                builder.UseCorrectCatchAllBehavior = useCorrectCatchAllBehavior.Value;
            }

            for (var i = 0; i < endpoints.Length; i++)
            {
                builder.AddEndpoint(endpoints[i]);
            }
            return builder.Build();
        }
    }
}
