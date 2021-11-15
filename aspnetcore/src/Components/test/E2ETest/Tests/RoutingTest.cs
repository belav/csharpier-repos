// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Runtime.InteropServices;
using BasicTestApp;
using BasicTestApp.RouterTest;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.E2ETesting;
using Microsoft.AspNetCore.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Components.E2ETest.Tests;

public class RoutingTest : ServerTestBase<ToggleExecutionModeServerFixture<Program>>
{
    public RoutingTest(
        BrowserFixture browserFixture,
        ToggleExecutionModeServerFixture<Program> serverFixture,
        ITestOutputHelper output)
        : base(browserFixture, serverFixture, output)
    {
    }

    protected override void InitializeAsyncCore()
    {
        Navigate(ServerPathBase, noReload: false);
        Browser.WaitUntilTestSelectorReady();
    }

    [Fact]
    public void CanArriveAtDefaultPage()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("This is the default page.", app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Default (matches all)", "Default with base-relative URL (matches all)");
    }

    [Fact]
    public void CanArriveAtDefaultPageWithoutTrailingSlash()
    {
        // This is a bit of a degenerate case because ideally devs would configure their
        // servers to enforce a canonical URL (with trailing slash) for the homepage.
        // But in case they don't want to, we need to handle it the same as if the URL does
        // have a trailing slash.
        SetUrlViaPushState("");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("This is the default page.", app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Default (matches all)", "Default with base-relative URL (matches all)");
    }

    [Fact]
    public void CanArriveAtPageWithParameters()
    {
        SetUrlViaPushState("/WithParameters/Name/Ghi/LastName/O'Jkl");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("Your full name is Ghi O'Jkl.", app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks();
    }

    [Fact]
    public void CanArriveAtPageWithNumberParameters()
    {
        var testInt = int.MinValue;
        var testLong = long.MinValue;
        var testDec = -2.33333m;
        var testDouble = -1.489d;
        var testFloat = -2.666f;

        SetUrlViaPushState($"/WithNumberParameters/{testInt}/{testLong}/{testDouble}/{testFloat}/{testDec}");

        var app = Browser.MountTestComponent<TestRouter>();
        var expected = $"Test parameters: {testInt} {testLong} {testDouble} {testFloat} {testDec}";

        Assert.Equal(expected, app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void CanArriveAtPageWithOptionalParametersProvided()
    {
        var testAge = 101;

        SetUrlViaPushState($"/WithOptionalParameters/{testAge}");

        var app = Browser.MountTestComponent<TestRouter>();
        var expected = $"Your age is {testAge}.";

        Assert.Equal(expected, app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void CanArriveAtPageWithOptionalParametersNotProvided()
    {
        SetUrlViaPushState($"/WithOptionalParameters?query=ignored");

        var app = Browser.MountTestComponent<TestRouter>();
        var expected = $"Your age is .";

        Assert.Equal(expected, app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void CanArriveAtPageWithCatchAllParameter()
    {
        SetUrlViaPushState("/WithCatchAllParameter/life/the/universe/and/everything%20%3D%2042?query=ignored");

        var app = Browser.MountTestComponent<TestRouter>();
        var expected = $"The answer: life/the/universe/and/everything = 42.";

        Assert.Equal(expected, app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void CanArriveAtNonDefaultPage()
    {
        SetUrlViaPushState("/Other");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("This is another page.", app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");
    }

    [Fact]
    public void CanArriveAtFallbackPageFromBadURI()
    {
        SetUrlViaPushState("/Oopsie_Daisies%20%This_Aint_A_Real_Page");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("Oops, that component wasn't found!", app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void CanFollowLinkToOtherPage()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Other")).Click();
        Browser.Equal("This is another page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");
    }

    [Fact]
    public void CanFollowLinkToOtherPageWithCtrlClick()
    {
        // On macOS we need to hold the command key not the control for opening a popup
        var key = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? Keys.Command : Keys.Control;

        try
        {
            SetUrlViaPushState("/");

            var app = Browser.MountTestComponent<TestRouter>();
            var button = app.FindElement(By.LinkText("Other"));

            new Actions(Browser).KeyDown(key).Click(button).Build().Perform();

            Browser.Equal(2, () => Browser.WindowHandles.Count);
        }
        finally
        {
            // Leaving the ctrl key up
            new Actions(Browser).KeyUp(key).Build().Perform();

            // Closing newly opened windows if a new one was opened
            while (Browser.WindowHandles.Count > 1)
            {
                Browser.SwitchTo().Window(Browser.WindowHandles.Last());
                Browser.Close();
            }

            // Needed otherwise Selenium tries to direct subsequent commands
            // to the tab that has already been closed
            Browser.SwitchTo().Window(Browser.WindowHandles.First());
        }
    }

    [Fact]
    public void CanFollowLinkToTargetBlankClick()
    {
        try
        {
            SetUrlViaPushState("/");

            var app = Browser.MountTestComponent<TestRouter>();

            app.FindElement(By.LinkText("Target (_blank)")).Click();

            Browser.Equal(2, () => Browser.WindowHandles.Count);
        }
        finally
        {
            // Closing newly opened windows if a new one was opened
            while (Browser.WindowHandles.Count > 1)
            {
                Browser.SwitchTo().Window(Browser.WindowHandles.Last());
                Browser.Close();
            }

            // Needed otherwise Selenium tries to direct subsequent commands
            // to the tab that has already been closed
            Browser.SwitchTo().Window(Browser.WindowHandles.First());
        }
    }

    [Fact]
    public void CanFollowLinkToOtherPageDoesNotOpenNewWindow()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();

        app.FindElement(By.LinkText("Other")).Click();

        Assert.Single(Browser.WindowHandles);
    }

    [Fact]
    public void CanFollowLinkToOtherPageWithBaseRelativeUrl()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Other with base-relative URL (matches all)")).Click();
        Browser.Equal("This is another page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");
    }

    [Fact]
    public void CanFollowLinkToEmptyStringHrefAsBaseRelativeUrl()
    {
        SetUrlViaPushState("/Other");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Default with base-relative URL (matches all)")).Click();
        Browser.Equal("This is the default page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Default (matches all)", "Default with base-relative URL (matches all)");
    }

    [Fact]
    public void CanFollowLinkToPageWithParameters()
    {
        SetUrlViaPushState("/Other");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("With parameters")).Click();
        Browser.Equal("Your full name is Abc .", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("With parameters");

        // Can add more parameters while remaining on same page
        app.FindElement(By.LinkText("With more parameters")).Click();
        Browser.Equal("Your full name is Abc McDef.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("With parameters", "With more parameters");

        // Can remove parameters while remaining on same page
        app.FindElement(By.LinkText("With parameters")).Click();
        Browser.Equal("Your full name is Abc .", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("With parameters");
    }

    [Fact]
    public void CanFollowLinkToDefaultPage()
    {
        SetUrlViaPushState("/Other");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Default (matches all)")).Click();
        Browser.Equal("This is the default page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Default (matches all)", "Default with base-relative URL (matches all)");
    }

    [Fact]
    public void CanFollowLinkToOtherPageWithQueryString()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Other with query")).Click();
        Browser.Equal("This is another page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Other", "Other with query");
    }

    [Fact]
    public void CanFollowLinkToDefaultPageWithQueryString()
    {
        SetUrlViaPushState("/Other");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Default with query")).Click();
        Browser.Equal("This is the default page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Default with query");
    }

    [Fact]
    public void CanFollowLinkToOtherPageWithHash()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Other with hash")).Click();
        Browser.Equal("This is another page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Other", "Other with hash");
    }

    [Fact]
    public void CanFollowLinkToDefaultPageWithHash()
    {
        SetUrlViaPushState("/Other");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Default with hash")).Click();
        Browser.Equal("This is the default page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Default with hash");
    }

    [Fact]
    public void CanFollowLinkToNotAComponent()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Not a component")).Click();
        Browser.Equal("Not a component!", () => Browser.Exists(By.Id("test-info")).Text);
    }

    [Fact]
    public void CanFollowLinkDefinedInOpenShadowRoot()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();

        // It's difficult to access elements within a shadow root using Selenium's regular APIs
        // Bypass this limitation by clicking the element via JavaScript
        var shadowHost = app.FindElement(By.TagName("custom-link-with-shadow-root"));
        ((IJavaScriptExecutor)Browser).ExecuteScript("arguments[0].shadowRoot.querySelector('a').click()", shadowHost);

        Browser.Equal("This is another page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");
    }

    [Fact]
    public void CanGoBackFromNotAComponent()
    {
        SetUrlViaPushState("/");

        // First go to some URL on the router
        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("Other")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));

        // Now follow a link out of the SPA entirely
        app.FindElement(By.LinkText("Not a component")).Click();
        Browser.Equal("Not a component!", () => Browser.Exists(By.Id("test-info")).Text);
        Browser.True(() => Browser.Url.EndsWith("/NotAComponent.html", StringComparison.Ordinal));

        // Now click back
        // Because of how the tests are structured with the router not appearing until the router
        // tests are selected, we can only observe the test selector being there, but this is enough
        // to show we did go back to the right place and the Blazor app started up
        Browser.Navigate().Back();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));
        Browser.WaitUntilTestSelectorReady();
    }

    [Fact]
    public void CanNavigateProgrammatically()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        var testSelector = Browser.WaitUntilTestSelectorReady();

        app.FindElement(By.Id("do-navigation")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));
        Browser.Equal("This is another page.", () => app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");

        // Because this was client-side navigation, we didn't lose the state in the test selector
        Assert.Equal(typeof(TestRouter).FullName, testSelector.SelectedOption.GetAttribute("value"));
    }

    [Fact]
    public void CanNavigateProgrammaticallyWithForceLoad()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        var testSelector = Browser.WaitUntilTestSelectorReady();

        app.FindElement(By.Id("do-navigation-forced")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));

        // Because this was a full-page load, our element references should no longer be valid
        Assert.Throws<StaleElementReferenceException>(() =>
        {
            testSelector.SelectedOption.GetAttribute("value");
        });
    }

    [Fact]
    public void CanNavigateProgrammaticallyValidateNoReplaceHistoryEntry()
    {
        // This test checks if default navigation does not replace Browser history entries
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        var testSelector = Browser.WaitUntilTestSelectorReady();

        app.FindElement(By.LinkText("Programmatic navigation cases")).Click();
        Browser.True(() => Browser.Url.EndsWith("/ProgrammaticNavigationCases", StringComparison.Ordinal));
        Browser.Contains("programmatic navigation", () => app.FindElement(By.Id("test-info")).Text);

        // We navigate to the /Other page
        // This will also test our new NavigatTo(string uri) overload (it should not replace the browser history)
        app.FindElement(By.Id("do-other-navigation")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");

        // After we press back, we should end up at the "/ProgrammaticNavigationCases" page so we know browser history has not been replaced
        // If history had been replaced we would have ended up at the "/" page
        Browser.Navigate().Back();
        Browser.True(() => Browser.Url.EndsWith("/ProgrammaticNavigationCases", StringComparison.Ordinal));
        AssertHighlightedLinks("Programmatic navigation cases");

        // For completeness, we will test if the normal NavigateTo(string uri, bool forceLoad) overload will also
        // NOT change the browser's history. So we basically repeat what we have done above.
        app.FindElement(By.Id("do-other-navigation2")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");

        Browser.Navigate().Back();
        Browser.True(() => Browser.Url.EndsWith("/ProgrammaticNavigationCases", StringComparison.Ordinal));
        AssertHighlightedLinks("Programmatic navigation cases");

        // Because this was client-side navigation, we didn't lose the state in the test selector
        Assert.Equal(typeof(TestRouter).FullName, testSelector.SelectedOption.GetAttribute("value"));

        app.FindElement(By.Id("do-other-navigation-forced")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));

        // We check if we had a force load
        Assert.Throws<StaleElementReferenceException>(() =>
            testSelector.SelectedOption.GetAttribute("value"));

        // But still we should be able to navigate back, and end up at the "/ProgrammaticNavigationCases" page
        Browser.Navigate().Back();
        Browser.True(() => Browser.Url.EndsWith("/ProgrammaticNavigationCases", StringComparison.Ordinal));
        Browser.WaitUntilTestSelectorReady();
    }

    [Fact]
    public void CanNavigateProgrammaticallyWithReplaceHistoryEntry()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        var testSelector = Browser.WaitUntilTestSelectorReady();

        app.FindElement(By.LinkText("Programmatic navigation cases")).Click();
        Browser.True(() => Browser.Url.EndsWith("/ProgrammaticNavigationCases", StringComparison.Ordinal));
        Browser.Contains("programmatic navigation", () => app.FindElement(By.Id("test-info")).Text);

        // We navigate to the /Other page, with "replace" enabled
        app.FindElement(By.Id("do-other-navigation-replacehistoryentry")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));
        AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");

        // After we press back, we should end up at the "/" page so we know browser history has been replaced
        // If history would not have been replaced we would have ended up at the "/ProgrammaticNavigationCases" page
        Browser.Navigate().Back();
        Browser.True(() => Browser.Url.EndsWith("/", StringComparison.Ordinal));
        AssertHighlightedLinks("Default (matches all)", "Default with base-relative URL (matches all)");

        // Because this was all with client-side navigation, we didn't lose the state in the test selector
        Assert.Equal(typeof(TestRouter).FullName, testSelector.SelectedOption.GetAttribute("value"));
    }

    [Fact]
    public void CanNavigateProgrammaticallyWithForceLoadAndReplaceHistoryEntry()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        var testSelector = Browser.WaitUntilTestSelectorReady();

        app.FindElement(By.LinkText("Programmatic navigation cases")).Click();
        Browser.True(() => Browser.Url.EndsWith("/ProgrammaticNavigationCases", StringComparison.Ordinal));
        Browser.Contains("programmatic navigation", () => app.FindElement(By.Id("test-info")).Text);

        // We navigate to the /Other page, with replacehistroyentry and forceload enabled
        app.FindElement(By.Id("do-other-navigation-forced-replacehistoryentry")).Click();
        Browser.True(() => Browser.Url.EndsWith("/Other", StringComparison.Ordinal));

        // We check if we had a force load
        Assert.Throws<StaleElementReferenceException>(() =>
            testSelector.SelectedOption.GetAttribute("value"));

        // After we press back, we should end up at the "/" page so we know browser history has been replaced
        Browser.Navigate().Back();
        Browser.True(() => Browser.Url.EndsWith("/", StringComparison.Ordinal));
        Browser.WaitUntilTestSelectorReady();
    }

    [Fact]
    public void ClickingAnchorWithNoHrefShouldNotNavigate()
    {
        SetUrlViaPushState("/");
        var initialUrl = Browser.Url;

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.Id("anchor-with-no-href")).Click();

        Assert.Equal(initialUrl, Browser.Url);
        AssertHighlightedLinks("Default (matches all)", "Default with base-relative URL (matches all)");
    }

    [Fact]
    public void UsingNavigationManagerWithoutRouterWorks()
    {
        var app = Browser.MountTestComponent<NavigationManagerComponent>();
        var initialUrl = Browser.Url;

        Browser.Equal(Browser.Url, () => app.FindElement(By.Id("test-info")).Text);
        var uri = SetUrlViaPushState("/mytestpath");
        Browser.Equal(uri, () => app.FindElement(By.Id("test-info")).Text);

        var jsExecutor = (IJavaScriptExecutor)Browser;
        jsExecutor.ExecuteScript("history.back()");

        Browser.Equal(initialUrl, () => app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void UriHelperCanReadAbsoluteUriIncludingHash()
    {
        var app = Browser.MountTestComponent<NavigationManagerComponent>();
        Browser.Equal(Browser.Url, () => app.FindElement(By.Id("test-info")).Text);

        var uri = "/mytestpath?my=query&another#some/hash?tokens";
        var expectedAbsoluteUri = $"{_serverFixture.RootUri}subdir{uri}";

        SetUrlViaPushState(uri);
        Browser.Equal(expectedAbsoluteUri, () => app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void CanArriveAtRouteWithExtension()
    {
        // This is an odd test, but it's primarily here to verify routing for routeablecomponentfrompackage isn't available due to
        // some unknown reason
        SetUrlViaPushState("/Default.html");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("This is the default page.", app.FindElement(By.Id("test-info")).Text);
        AssertHighlightedLinks("With extension");
    }

    [Fact]
    public void RoutingToComponentOutsideMainAppDoesNotWork()
    {
        SetUrlViaPushState("/routeablecomponentfrompackage.html");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("Oops, that component wasn't found!", app.FindElement(By.Id("test-info")).Text);
    }

    [Fact]
    public void RoutingToComponentOutsideMainAppWorksWithAdditionalAssemblySpecified()
    {
        SetUrlViaPushState("/routeablecomponentfrompackage.html");

        var app = Browser.MountTestComponent<TestRouterWithAdditionalAssembly>();
        Assert.Contains("This component, including the CSS and image required to produce its", app.FindElement(By.CssSelector("div.special-style")).Text);
    }

    [Fact]
    public void ResetsScrollPositionWhenPerformingInternalNavigation_LinkClick()
    {
        SetUrlViaPushState("/LongPage1");
        var app = Browser.MountTestComponent<TestRouter>();
        Browser.Equal("This is a long page you can scroll.", () => app.FindElement(By.Id("test-info")).Text);
        BrowserScrollY = 500;
        Browser.True(() => BrowserScrollY > 300); // Exact position doesn't matter

        app.FindElement(By.LinkText("Long page 2")).Click();
        Browser.Equal("This is another long page you can scroll.", () => app.FindElement(By.Id("test-info")).Text);
        Browser.Equal(0, () => BrowserScrollY);
    }

    [Fact]
    public void ResetsScrollPositionWhenPerformingInternalNavigation_ProgrammaticNavigation()
    {
        SetUrlViaPushState("/LongPage1");
        var app = Browser.MountTestComponent<TestRouter>();
        Browser.Equal("This is a long page you can scroll.", () => app.FindElement(By.Id("test-info")).Text);
        BrowserScrollY = 500;
        Browser.True(() => BrowserScrollY > 300); // Exact position doesn't matter

        app.FindElement(By.Id("go-to-longpage2")).Click();
        Browser.Equal("This is another long page you can scroll.", () => app.FindElement(By.Id("test-info")).Text);
        Browser.Equal(0, () => BrowserScrollY);
    }

    [Fact]
    public void PreventDefault_CanBlockNavigation_ForInternalNavigation_PreventDefaultTarget()
        => PreventDefault_CanBlockNavigation("internal", "target");

    [Fact]
    public void PreventDefault_CanBlockNavigation_ForExternalNavigation_PreventDefaultAncestor()
        => PreventDefault_CanBlockNavigation("external", "ancestor");

    [Theory]
    [InlineData("external", "target")]
    [InlineData("external", "descendant")]
    [InlineData("internal", "ancestor")]
    [InlineData("internal", "descendant")]
    public virtual void PreventDefault_CanBlockNavigation(string navigationType, string whereToPreventDefault)
    {
        SetUrlViaPushState("/PreventDefaultCases");
        var app = Browser.MountTestComponent<TestRouter>();
        var preventDefaultToggle = app.FindElement(By.CssSelector($".prevent-default .{whereToPreventDefault}"));
        var linkElement = app.FindElement(By.Id($"{navigationType}-navigation"));
        var counterButton = app.FindElement(By.ClassName("counter-button"));
        if (whereToPreventDefault == "descendant")
        {
            // We're testing clicks on the link's descendant element
            linkElement = linkElement.FindElement(By.TagName("span"));
        }

        // If preventDefault is on, then navigation does not occur
        preventDefaultToggle.Click();
        linkElement.Click();

        // We check that no navigation ocurred by observing that we can still use the counter
        counterButton.Click();
        Browser.Equal("Counter: 1", () => counterButton.Text);

        // Now if we toggle preventDefault back off, then navigation will occur
        preventDefaultToggle.Click();
        linkElement.Click();

        if (navigationType == "external")
        {
            Browser.Equal("about:blank", () => Browser.Url);
        }
        else
        {
            Browser.Equal("This is another page.", () => app.FindElement(By.Id("test-info")).Text);
            AssertHighlightedLinks("Other", "Other with base-relative URL (matches all)");
        }
    }

    [Fact]
    public void OnNavigate_CanRenderLoadingFragment()
    {
        var app = Browser.MountTestComponent<TestRouterWithOnNavigate>();

        SetUrlViaPushState("/LongPage1");

        Browser.Exists(By.Id("loading-banner"));
    }

    [Fact]
    public void OnNavigate_CanCancelCallback()
    {
        var app = Browser.MountTestComponent<TestRouterWithOnNavigate>();

        // Navigating from one page to another should
        // cancel the previous OnNavigate Task
        SetUrlViaPushState("/LongPage2");
        SetUrlViaPushState("/LongPage1");

        AssertDidNotLog("I'm not happening...");
    }

    [Fact]
    public void OnNavigate_CanRenderUIForExceptions()
    {
        var app = Browser.MountTestComponent<TestRouterWithOnNavigate>();

        SetUrlViaPushState("/Other");

        var errorUiElem = Browser.Exists(By.Id("blazor-error-ui"), TimeSpan.FromSeconds(10));
        Assert.NotNull(errorUiElem);
    }

    [Fact]
    public void OnNavigate_CanRenderUIForSyncExceptions()
    {
        var app = Browser.MountTestComponent<TestRouterWithOnNavigate>();

        // Should capture exception from synchronously thrown
        SetUrlViaPushState("/WithLazyAssembly");

        var errorUiElem = Browser.Exists(By.Id("blazor-error-ui"), TimeSpan.FromSeconds(10));
        Assert.NotNull(errorUiElem);
    }

    [Fact]
    public void OnNavigate_DoesNotRenderWhileOnNavigateExecuting()
    {
        var app = Browser.MountTestComponent<TestRouterWithOnNavigate>();

        // Navigate to a route
        SetUrlViaPushState("/WithParameters/name/Abc");

        // Click the button to trigger a re-render
        var button = app.FindElement(By.Id("trigger-rerender"));
        button.Click();

        // Assert that the parameter route didn't render
        Browser.DoesNotExist(By.Id("test-info"));

        // Navigate to another page to cancel the previous `OnNavigateAsync`
        // task and trigger a re-render on its completion
        SetUrlViaPushState("/LongPage1");

        // Confirm that the route was rendered
        Browser.Equal("This is a long page you can scroll.", () => app.FindElement(By.Id("test-info")).Text);
    }

    [Theory]
    [InlineData("/WithParameters/Name/Ñoño ñi/LastName/O'Jkl")]
    [InlineData("/WithParameters/Name/[Ñoño ñi]/LastName/O'Jkl")]
    [InlineData("/other?abc=Ñoño ñi")]
    [InlineData("/other?abc=[Ñoño ñi]")]
    public void CanArriveAtPageWithSpecialURL(string relativeUrl)
    {
        SetUrlViaPushState(relativeUrl, true);
        var errorUi = Browser.Exists(By.Id("blazor-error-ui"));
        Browser.Equal("none", () => errorUi.GetCssValue("display"));
    }

    [Fact]
    public void FocusOnNavigation_SetsFocusToMatchingElement()
    {
        // Applies focus on initial load
        SetUrlViaPushState("/");
        var app = Browser.MountTestComponent<TestRouter>();
        Browser.True(() => GetFocusedElement().Text == "This is the default page.");

        // Updates focus after navigation to regular page
        app.FindElement(By.LinkText("Other")).Click();
        Browser.True(() => GetFocusedElement().Text == "This is another page.");

        // If there's no matching element, we leave the focus unchanged
        app.FindElement(By.Id("with-lazy-assembly")).Click();
        Browser.Exists(By.Id("use-package-button"));
        Browser.Equal("a", () => GetFocusedElement().TagName);

        // No errors from lack of matching element - app still functions
        app.FindElement(By.LinkText("Other")).Click();
        Browser.True(() => GetFocusedElement().Text == "This is another page.");

        IWebElement GetFocusedElement()
            => Browser.SwitchTo().ActiveElement();
    }

    [Fact]
    public void CanArriveAtQueryStringPageWithNoQuery()
    {
        SetUrlViaPushState("/WithQueryParameters/Abc");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("Hello Abc .", app.FindElement(By.Id("test-info")).Text);
        Assert.Equal("0", app.FindElement(By.Id("value-QueryInt")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateTimeValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableTimeOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-StringValue")).Text);
        Assert.Equal("0 values ()", app.FindElement(By.Id("value-LongValues")).Text);

        AssertHighlightedLinks("With query parameters (none)");
    }

    [Fact]
    public void CanArriveAtQueryStringPageWithStringQuery()
    {
        SetUrlViaPushState("/WithQueryParameters/Abc?stringvalue=Hello+there");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("Hello Abc .", app.FindElement(By.Id("test-info")).Text);
        Assert.Equal("0", app.FindElement(By.Id("value-QueryInt")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateTimeValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableTimeOnlyValue")).Text);
        Assert.Equal("Hello there", app.FindElement(By.Id("value-StringValue")).Text);
        Assert.Equal("0 values ()", app.FindElement(By.Id("value-LongValues")).Text);

        AssertHighlightedLinks("With query parameters (none)", "With query parameters (passing string value)");
    }

    [Fact]
    public void CanArriveAtQueryStringPageWithDateTimeQuery()
    {
        var dateTime = new DateTime(2000, 1, 2, 3, 4, 5, 6);
        var dateOnly = new DateOnly(2000, 1, 2);
        var timeOnly = new TimeOnly(3, 4, 5, 6);
        SetUrlViaPushState($"/WithQueryParameters/Abc?NullableDateTimeValue=2000-01-02%2003:04:05&NullableDateOnlyValue=2000-01-02&NullableTimeOnlyValue=03:04:05");

        var app = Browser.MountTestComponent<TestRouter>();
        Assert.Equal("Hello Abc .", app.FindElement(By.Id("test-info")).Text);
        Assert.Equal("0", app.FindElement(By.Id("value-QueryInt")).Text);
        Assert.Equal(dateTime.ToString("hh:mm:ss on yyyy-MM-dd", CultureInfo.InvariantCulture), app.FindElement(By.Id("value-NullableDateTimeValue")).Text);
        Assert.Equal(dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), app.FindElement(By.Id("value-NullableDateOnlyValue")).Text);
        Assert.Equal(timeOnly.ToString("hh:mm:ss", CultureInfo.InvariantCulture), app.FindElement(By.Id("value-NullableTimeOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-StringValue")).Text);
        Assert.Equal("0 values ()", app.FindElement(By.Id("value-LongValues")).Text);

        AssertHighlightedLinks("With query parameters (none)", "With query parameters (passing Date Time values)");
    }

    [Fact]
    public void CanNavigateToQueryStringPageWithNoQuery()
    {
        SetUrlViaPushState("/");

        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("With query parameters (none)")).Click();

        Assert.Equal("Hello Abc .", app.FindElement(By.Id("test-info")).Text);
        Assert.Equal("0", app.FindElement(By.Id("value-QueryInt")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateTimeValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableTimeOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-StringValue")).Text);
        Assert.Equal("0 values ()", app.FindElement(By.Id("value-LongValues")).Text);

        AssertHighlightedLinks("With query parameters (none)");
    }

    [Fact]
    public void CanNavigateBetweenPagesWithQueryStrings()
    {
        SetUrlViaPushState("/");

        // Navigate to a page with querystring
        var app = Browser.MountTestComponent<TestRouter>();
        app.FindElement(By.LinkText("With query parameters (passing string value)")).Click();

        Browser.Equal("Hello Abc .", () => app.FindElement(By.Id("test-info")).Text);
        Assert.Equal("0", app.FindElement(By.Id("value-QueryInt")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateTimeValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableTimeOnlyValue")).Text);
        Assert.Equal("Hello there", app.FindElement(By.Id("value-StringValue")).Text);
        Assert.Equal("0 values ()", app.FindElement(By.Id("value-LongValues")).Text);
        var instanceId = app.FindElement(By.Id("instance-id")).Text;
        Assert.True(!string.IsNullOrWhiteSpace(instanceId));

        AssertHighlightedLinks("With query parameters (none)", "With query parameters (passing string value)");

        // We can also navigate to a different query while retaining the same component instance
        app.FindElement(By.LinkText("With IntValue and LongValues")).Click();
        Browser.Equal("123", () => app.FindElement(By.Id("value-QueryInt")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateTimeValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableTimeOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-StringValue")).Text);
        Assert.Equal("3 values (50, 100, -20)", app.FindElement(By.Id("value-LongValues")).Text);
        Assert.Equal(instanceId, app.FindElement(By.Id("instance-id")).Text);
        AssertHighlightedLinks("With query parameters (none)");

        // We can also click back to go the preceding query while retaining the same component instance
        Browser.Navigate().Back();
        Browser.Equal("0", () => app.FindElement(By.Id("value-QueryInt")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateTimeValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableDateOnlyValue")).Text);
        Assert.Equal(string.Empty, app.FindElement(By.Id("value-NullableTimeOnlyValue")).Text);
        Assert.Equal("Hello there", app.FindElement(By.Id("value-StringValue")).Text);
        Assert.Equal("0 values ()", app.FindElement(By.Id("value-LongValues")).Text);
        Assert.Equal(instanceId, app.FindElement(By.Id("instance-id")).Text);
        AssertHighlightedLinks("With query parameters (none)", "With query parameters (passing string value)");
    }

    private long BrowserScrollY
    {
        get => (long)((IJavaScriptExecutor)Browser).ExecuteScript("return window.scrollY");
        set => ((IJavaScriptExecutor)Browser).ExecuteScript($"window.scrollTo(0, {value})");
    }

    private string SetUrlViaPushState(string relativeUri, bool forceLoad = false)
    {
        var pathBaseWithoutHash = ServerPathBase.Split('#')[0];
        var jsExecutor = (IJavaScriptExecutor)Browser;
        var absoluteUri = new Uri(_serverFixture.RootUri, $"{pathBaseWithoutHash}{relativeUri}");
        jsExecutor.ExecuteScript($"Blazor.navigateTo('{absoluteUri.ToString().Replace("'", "\\'")}', {(forceLoad ? "true" : "false")})");

        return absoluteUri.AbsoluteUri;
    }

    private void AssertDidNotLog(params string[] messages)
    {
        var log = Browser.Manage().Logs.GetLog(LogType.Browser);
        foreach (var message in messages)
        {
            Assert.DoesNotContain(log, entry => entry.Message.Contains(message));
        }
    }

    private void AssertHighlightedLinks(params string[] linkTexts)
    {
        Browser.Equal(linkTexts, () => Browser
            .FindElements(By.CssSelector("a.active"))
            .Select(x => x.Text));
    }
}
