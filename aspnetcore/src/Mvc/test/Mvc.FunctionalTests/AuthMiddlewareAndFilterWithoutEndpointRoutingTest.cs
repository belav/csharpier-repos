// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Mvc.FunctionalTests;

public class AuthMiddlewareAndFilterWithoutEndpointRoutingTest
    : AuthMiddlewareAndFilterTestBase<SecurityWebSite.StartupWithGlobalAuthFilterWithoutEndpointRouting>
{
    public AuthMiddlewareAndFilterWithoutEndpointRoutingTest(
        MvcTestFixture<SecurityWebSite.StartupWithGlobalAuthFilterWithoutEndpointRouting> fixture
    )
        : base(fixture) { }
}
