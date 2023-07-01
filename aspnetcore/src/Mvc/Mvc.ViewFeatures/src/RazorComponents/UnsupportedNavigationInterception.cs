using Microsoft.AspNetCore.Components.Routing;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

internal sealed class UnsupportedNavigationInterception : INavigationInterception
{
    public Task EnableNavigationInterceptionAsync()
    {
        throw new InvalidOperationException(
            "Navigation interception calls cannot be issued during server-side prerendering, because the page has not yet loaded in the browser. "
                + "Prerendered components must wrap any navigation interception calls in conditional logic to ensure those interop calls are not attempted during prerendering."
        );
    }
}
