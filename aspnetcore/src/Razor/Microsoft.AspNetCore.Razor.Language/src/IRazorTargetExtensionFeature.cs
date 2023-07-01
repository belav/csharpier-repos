using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language;

public interface IRazorTargetExtensionFeature : IRazorEngineFeature
{
    ICollection<ICodeTargetExtension> TargetExtensions { get; }
}
