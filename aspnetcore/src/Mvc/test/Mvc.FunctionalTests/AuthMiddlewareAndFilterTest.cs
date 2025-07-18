// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Mvc.FunctionalTests;

public class AuthMiddlewareAndFilterTest
    : AuthMiddlewareAndFilterTestBase<SecurityWebSite.StartupWithGlobalAuthFilter>
{
    public AuthMiddlewareAndFilterTest(
        MvcTestFixture<SecurityWebSite.StartupWithGlobalAuthFilter> fixture
    )
        : base(fixture) { }
}
