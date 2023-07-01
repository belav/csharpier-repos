using Microsoft.AspNetCore.Razor.Language;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions.Version2_X;

internal class ExtensionInitializer : RazorExtensionInitializer
{
    public override void Initialize(RazorProjectEngineBuilder builder)
    {
        RazorExtensions.Register(builder);
    }
}
