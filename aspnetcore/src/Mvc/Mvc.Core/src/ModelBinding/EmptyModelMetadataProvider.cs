using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// A <see cref="DefaultBindingMetadataProvider"/> that represents an empty model.
/// </summary>
public class EmptyModelMetadataProvider : DefaultModelMetadataProvider
{
    /// <summary>
    /// Initializes a new <see cref="EmptyModelMetadataProvider"/>.
    /// </summary>
    public EmptyModelMetadataProvider()
        : base(
            new DefaultCompositeMetadataDetailsProvider(new List<IMetadataDetailsProvider>()),
            new OptionsAccessor()
        ) { }

    private sealed class OptionsAccessor : IOptions<MvcOptions>
    {
        public MvcOptions Value { get; } = new MvcOptions();
    }
}
