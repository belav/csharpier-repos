// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNetCore.Routing.Matching
{
    public class RouteMatcherConformanceTest : FullFeaturedMatcherConformanceTest
    {
        // https://github.com/dotnet/aspnetcore/issues/18677
        //
        [Theory]
        [InlineData("/middleware", 1)]
        [InlineData("/middleware/test", 1)]
        [InlineData("/middleware/test1/test2", 1)]
        [InlineData("/bill/boga", 0)]
        public async Task Match_Regression_1867(string path, int endpointIndex)
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

            var matcher = CreateMatcher(endpoints);
            var httpContext = CreateContext(path);

            // Act
            await matcher.MatchAsync(httpContext);

            // Assert
            MatcherAssert.AssertMatch(httpContext, expected, ignoreValues: true);
        }

        internal override Matcher CreateMatcher(params RouteEndpoint[] endpoints)
        {
            var builder = new RouteMatcherBuilder();
            for (var i = 0; i < endpoints.Length; i++)
            {
                builder.AddEndpoint(endpoints[i]);
            }
            return builder.Build();
        }
    }
}
