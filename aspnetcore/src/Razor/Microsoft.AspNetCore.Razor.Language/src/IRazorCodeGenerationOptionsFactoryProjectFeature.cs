using System;

namespace Microsoft.AspNetCore.Razor.Language;

internal interface IRazorCodeGenerationOptionsFactoryProjectFeature : IRazorProjectEngineFeature
{
    RazorCodeGenerationOptions Create(
        string fileKind,
        Action<RazorCodeGenerationOptionsBuilder> configure
    );
}
