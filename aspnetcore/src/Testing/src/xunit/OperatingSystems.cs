using System;

namespace Microsoft.AspNetCore.Testing;

[Flags]
public enum OperatingSystems
{
    Linux = 1,
    MacOSX = 2,
    Windows = 4,
}
