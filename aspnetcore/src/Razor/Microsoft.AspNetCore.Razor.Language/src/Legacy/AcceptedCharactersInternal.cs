using System;

namespace Microsoft.AspNetCore.Razor.Language.Legacy;

[Flags]
internal enum AcceptedCharactersInternal
{
    None = 0,
    NewLine = 1,
    Whitespace = 2,

    NonWhitespace = 4,

    AllWhitespace = NewLine | Whitespace,
    Any = AllWhitespace | NonWhitespace,

    AnyExceptNewline = NonWhitespace | Whitespace
}
