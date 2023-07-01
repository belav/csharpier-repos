using System.Collections.Generic;

namespace Microsoft.AspNetCore.Razor.Language;

public interface ITagHelperFeature : IRazorEngineFeature
{
    IReadOnlyList<TagHelperDescriptor> GetDescriptors();
}
