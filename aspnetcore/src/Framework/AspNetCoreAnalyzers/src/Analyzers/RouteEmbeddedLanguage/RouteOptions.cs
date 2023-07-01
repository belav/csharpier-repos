using System;

namespace Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage;

[Flags]
internal enum RouteOptions
{
    /// <summary>
    /// HTTP route. Used to match endpoints for Minimal API, MVC, SignalR, gRPC, etc.
    /// </summary>
    Http = 0,

    /// <summary>
    /// Component route. Used to match Razor components for Blazor.
    /// </summary>
    Component = 1,
}
