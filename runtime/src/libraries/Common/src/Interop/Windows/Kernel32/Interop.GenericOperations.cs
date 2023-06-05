partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Kernel32
    {
        partial internal static class GenericOperations
        {
            internal const int GENERIC_READ = unchecked((int)0x80000000);
            internal const int GENERIC_WRITE = 0x40000000;
        }
    }
}
