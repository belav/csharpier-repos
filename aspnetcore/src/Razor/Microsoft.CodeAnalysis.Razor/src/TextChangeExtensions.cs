using System;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Razor;

internal static class TextChangeExtensions
{
    public static SourceChange AsSourceChange(this TextChange textChange)
    {
        return new SourceChange(textChange.Span.AsSourceSpan(), textChange.NewText);
    }
}
