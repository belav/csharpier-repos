using System;

namespace Microsoft.AspNetCore.Testing;

public static class StringExtensions
{
    public static string EscapeNonPrintable(this string s)
    {
        var ellipsis = s.Length > 128 ? "..." : string.Empty;
        return s.Substring(0, Math.Min(128, s.Length))
                .Replace("\r", @"\x0D")
                .Replace("\n", @"\x0A")
                .Replace("\0", @"\x00")
                .Replace("\x80", @"\x80") + ellipsis;
    }
}
