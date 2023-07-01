using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language;

namespace Microsoft.CodeAnalysis.Razor;

public interface IMetadataReferenceFeature : IRazorEngineFeature
{
    IReadOnlyList<MetadataReference> References { get; }
}
