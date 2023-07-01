using System;

namespace Microsoft.AspNetCore.Razor.Language;

internal interface IRazorParserOptionsFactoryProjectFeature : IRazorProjectEngineFeature
{
    RazorParserOptions Create(string fileKind, Action<RazorParserOptionsBuilder> configure);
}
