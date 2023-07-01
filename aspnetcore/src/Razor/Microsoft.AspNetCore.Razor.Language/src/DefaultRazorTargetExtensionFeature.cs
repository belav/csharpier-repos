using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Microsoft.AspNetCore.Razor.Language;

internal class DefaultRazorTargetExtensionFeature
    : RazorEngineFeatureBase,
        IRazorTargetExtensionFeature
{
    public ICollection<ICodeTargetExtension> TargetExtensions { get; } =
        new List<ICodeTargetExtension>();
}
