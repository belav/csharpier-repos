using System;

namespace Microsoft.AspNetCore.BrowserTesting;

[Flags]
public enum BrowserKind
{
    Chromium = 1,
    Firefox = 2,
    Webkit = 4
}
