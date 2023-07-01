using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language;

namespace Microsoft.CodeAnalysis.Razor;

public sealed class DefaultMetadataReferenceFeature
    : RazorEngineFeatureBase,
        IMetadataReferenceFeature
{
    public IReadOnlyList<MetadataReference> References { get; set; }
}
