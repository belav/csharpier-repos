using Microsoft.CodeAnalysis.Text;

namespace Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage.Infrastructure.VirtualChars;

internal static class TextLineExtensions
{
    public static int? GetFirstNonWhitespaceOffset(this TextLine line)
    {
        var text = line.Text;
        if (text != null)
        {
            for (var i = line.Start; i < line.End; i++)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    return i - line.Start;
                }
            }
        }

        return null;
    }
}
