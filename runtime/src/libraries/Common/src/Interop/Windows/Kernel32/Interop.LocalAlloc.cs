// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        internal const uint LMEM_FIXED = 0x0000;
        internal const uint LMEM_MOVEABLE = 0x0002;
        internal const uint LMEM_ZEROINIT = 0x0040;

        [LibraryImport(Libraries.Kernel32)]
        partial internal static IntPtr LocalAlloc(uint uFlags, nuint uBytes);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static IntPtr LocalReAlloc(IntPtr hMem, nuint uBytes, uint uFlags);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static IntPtr LocalFree(IntPtr hMem);
    }
}
