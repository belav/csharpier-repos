using System;

namespace Microsoft.AspNetCore.Razor.Language.Legacy;

[Flags]
internal enum BalancingModes
{
    None = 0,
    BacktrackOnFailure = 1,
    NoErrorOnFailure = 2,
    AllowCommentsAndTemplates = 4,
    AllowEmbeddedTransitions = 8,
    StopAtEndOfLine = 16,
}
