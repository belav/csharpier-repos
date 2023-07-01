using System;

namespace Microsoft.AspNetCore.Razor.Language;

[Obsolete("In Razor 2.1 and newer, use RazorCodeDocument.GetParserOptions().")]
public interface IRazorParserOptionsFeature : IRazorEngineFeature
{
    RazorParserOptions GetOptions();
}
