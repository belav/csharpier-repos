using System.Collections.Generic;

namespace Microsoft.AspNetCore.Razor.Language;

public interface IRazorDirectiveFeature : IRazorEngineFeature
{
    ICollection<DirectiveDescriptor> Directives { get; }
}
