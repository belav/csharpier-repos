// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe IntPtr CreateThreadpoolTimer(
            delegate* unmanaged<void*, void*, void*, void> pfnti,
            IntPtr pv,
            IntPtr pcbe
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe IntPtr SetThreadpoolTimer(
            IntPtr pti,
            long* pftDueTime,
            uint msPeriod,
            uint msWindowLength
        );
    }
}
