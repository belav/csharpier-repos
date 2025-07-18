// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Identity.DefaultUI.WebSite;
using Identity.DefaultUI.WebSite.Data;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Bootstrap4Tests;

public class Bootstrap4LoginTests : LoginTests<ApplicationUserStartup, ApplicationDbContext>
{
    public Bootstrap4LoginTests(
        ServerFactory<ApplicationUserStartup, ApplicationDbContext> serverFactory
    )
        : base(serverFactory)
    {
        serverFactory.BootstrapFrameworkVersion = "V4";
    }
}
