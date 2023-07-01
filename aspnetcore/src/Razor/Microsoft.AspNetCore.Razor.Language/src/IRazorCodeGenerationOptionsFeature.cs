using System;

namespace Microsoft.AspNetCore.Razor.Language;

[Obsolete("In Razor 2.1 and newer, use RazorCodeDocument.GetCodeGenerationOptions().")]
public interface IRazorCodeGenerationOptionsFeature : IRazorEngineFeature
{
    RazorCodeGenerationOptions GetOptions();
}
