using BasicTestApp;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.E2ETesting;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Components.E2ETest.Tests;

public sealed class ComponentRenderingTest : ComponentRenderingTestBase
{
    public ComponentRenderingTest(
        BrowserFixture browserFixture,
        ToggleExecutionModeServerFixture<Program> serverFixture,
        ITestOutputHelper output
    )
        : base(browserFixture, serverFixture, output) { }

    [Fact]
    public void CanDispatchAsyncWorkToSyncContext()
    {
        var appElement = Browser.MountTestComponent<DispatchingComponent>();
        var result = appElement.FindElement(By.Id("result"));

        appElement.FindElement(By.Id("run-async-with-dispatch")).Click();

        Browser.Equal("First Second Third Fourth Fifth", () => result.Text);
    }
}
