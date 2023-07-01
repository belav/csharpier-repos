using OpenQA.Selenium;

namespace Microsoft.AspNetCore.Components.E2ETest;

internal static class WebDriverExtensions
{
    public static void Navigate(
        this IWebDriver browser,
        Uri baseUri,
        string relativeUrl,
        bool noReload
    )
    {
        var absoluteUrl = new Uri(baseUri, relativeUrl);

        browser.Navigate().GoToUrl("about:blank");
        browser.Navigate().GoToUrl(absoluteUrl);
    }
}
