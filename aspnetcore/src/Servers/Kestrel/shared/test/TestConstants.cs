using System;

namespace Microsoft.AspNetCore.Testing;

public class TestConstants
{
    public const int EOF = -4095;
    public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
}
