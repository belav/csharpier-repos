using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

internal sealed class HttpNavigationManager : NavigationManager, IHostEnvironmentNavigationManager
{
    void IHostEnvironmentNavigationManager.Initialize(string baseUri, string uri) =>
        Initialize(baseUri, uri);

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        throw new NavigationException(uri);
    }
}
