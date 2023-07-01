using System.Collections.Generic;

namespace Microsoft.AspNetCore.Razor.Language;

public interface IImportProjectFeature : IRazorProjectEngineFeature
{
    IReadOnlyList<RazorProjectItem> GetImports(RazorProjectItem projectItem);
}
