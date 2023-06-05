// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class WinMM
    {
        internal const int MMIO_FINDRIFF = 0x00000020;

        [LibraryImport(Libraries.WinMM)]
        partial internal static unsafe int mmioDescend(
            IntPtr hMIO,
            MMCKINFO* lpck,
            MMCKINFO* lcpkParent,
            int flags
        );
    }
}
