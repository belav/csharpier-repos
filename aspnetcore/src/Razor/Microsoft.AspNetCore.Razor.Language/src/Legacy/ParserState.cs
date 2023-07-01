using System;

namespace Microsoft.AspNetCore.Razor.Language.Legacy;

internal enum ParserState
{
    CData,
    CodeTransition,
    DoubleTransition,
    EOF,
    MarkupComment,
    MarkupText,
    Misc,
    RazorComment,
    SpecialTag,
    Tag,
    Unknown,
    XmlPI,
}
