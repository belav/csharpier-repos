// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.Http;
using Components.TestServer.RazorComponents;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.E2ETesting;
using Microsoft.AspNetCore.InternalTesting;
using OpenQA.Selenium;
using TestServer;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Components.E2ETests.ServerRenderingTests;

public class RenderingTest
    : ServerTestBase<BasicTestAppServerSiteFixture<RazorComponentEndpointsStartup<App>>>
{
    public RenderingTest(
        BrowserFixture browserFixture,
        BasicTestAppServerSiteFixture<RazorComponentEndpointsStartup<App>> serverFixture,
        ITestOutputHelper output
    )
        : base(browserFixture, serverFixture, output) { }

    public override Task InitializeAsync() => InitializeAsync(BrowserFixture.StreamingContext);

    [Fact]
    [QuarantinedTest("https://github.com/dotnet/aspnetcore/issues/49975")]
    public void CanRenderLargeComponentsWithServerRenderMode()
    {
        Navigate($"{ServerPathBase}/large-html-server");
        var result = new string('*', 50000);

        Assert.Equal(result, Browser.FindElement(By.Id("webassembly-prerender")).Text);
        Assert.Equal(result, Browser.FindElement(By.Id("server-prerender")).Text);
        Assert.Equal(result, Browser.FindElement(By.Id("server-prerender")).Text);
    }

    [Fact]
    public async Task CanUseHttpContextRequestAndResponse()
    {
        Navigate($"{ServerPathBase}/httpcontext");
        Browser.Equal("GET", () => Browser.FindElement(By.Id("request-method")).Text);
        Browser.Equal("/httpcontext", () => Browser.FindElement(By.Id("request-path")).Text);

        // We can't see the response status code using Selenium, so make a direct request
        var response = await new HttpClient().GetAsync(Browser.Url);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
