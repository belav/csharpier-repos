using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Razor;

internal static class TextSpanExtensions
{
    public static SourceSpan AsSourceSpan(this TextSpan textSpan)
    {
        return new SourceSpan(
            filePath: null,
            absoluteIndex: textSpan.Start,
            lineIndex: -1,
            characterIndex: -1,
            length: textSpan.Length
        );
    }
}
