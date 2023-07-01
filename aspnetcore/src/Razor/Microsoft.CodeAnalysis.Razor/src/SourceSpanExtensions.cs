using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Razor;

internal static class SourceSpanExtensions
{
    public static TextSpan AsTextSpan(this SourceSpan sourceSpan)
    {
        return new TextSpan(sourceSpan.AbsoluteIndex, sourceSpan.Length);
    }
}
