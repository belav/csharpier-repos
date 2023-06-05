partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Crypt32
    {
        internal enum ChainEngine : int
        {
            HCCE_CURRENT_USER = 0x0,
            HCCE_LOCAL_MACHINE = 0x1,
        }
    }
}
