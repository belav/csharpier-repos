using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Razor.Language;

internal class DefaultImportProjectFeature : RazorProjectEngineFeatureBase, IImportProjectFeature
{
    public IReadOnlyList<RazorProjectItem> GetImports(RazorProjectItem projectItem) =>
        Array.Empty<RazorProjectItem>();
}
