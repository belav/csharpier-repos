using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Components;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions;

internal class ExtensionInitializer : RazorExtensionInitializer
{
    public override void Initialize(RazorProjectEngineBuilder builder)
    {
        RazorExtensions.Register(builder);
    }
}
