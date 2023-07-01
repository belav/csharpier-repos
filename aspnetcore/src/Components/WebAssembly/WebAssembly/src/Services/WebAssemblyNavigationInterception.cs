using Microsoft.AspNetCore.Components.Routing;
using Interop = Microsoft.AspNetCore.Components.Web.BrowserNavigationManagerInterop;
using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Components.WebAssembly.Services;

internal sealed class WebAssemblyNavigationInterception : INavigationInterception
{
    public static readonly WebAssemblyNavigationInterception Instance =
        new WebAssemblyNavigationInterception();

    public Task EnableNavigationInterceptionAsync()
    {
        DefaultWebAssemblyJSRuntime.Instance.InvokeVoid(Interop.EnableNavigationInterception);
        return Task.CompletedTask;
    }
}
