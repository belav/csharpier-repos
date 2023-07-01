using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Razor;

namespace Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

internal sealed class LazyMetadataReferenceFeature : IMetadataReferenceFeature
{
    private readonly RazorReferenceManager _referenceManager;

    public LazyMetadataReferenceFeature(RazorReferenceManager referenceManager)
    {
        _referenceManager = referenceManager;
    }

    /// <remarks>
    /// Invoking <see cref="RazorReferenceManager.CompilationReferences"/> ensures that compilation
    /// references are lazily evaluated.
    /// </remarks>
    public IReadOnlyList<MetadataReference> References => _referenceManager.CompilationReferences;

    public RazorEngine? Engine { get; set; }
}
