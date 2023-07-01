using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

internal sealed class InvokedRenderModes
{
    public InvokedRenderModes(Mode mode)
    {
        Value = mode;
    }

    public Mode Value { get; set; }

    /// <summary>
    /// Tracks <see cref="RenderMode"/> for components.
    /// </summary>
    internal enum Mode
    {
        None,
        Server,
        WebAssembly,

        /// <summary>
        /// Tracks an app that has both components rendered both on the Server and WebAssembly.
        /// </summary>
        ServerAndWebAssembly
    }
}
