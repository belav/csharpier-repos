using System;

namespace Microsoft.AspNetCore.Testing;

[Flags]
public enum RuntimeFrameworks
{
    None = 0,
    Mono = 1 << 0,
    CLR = 1 << 1,
    CoreCLR = 1 << 2
}
