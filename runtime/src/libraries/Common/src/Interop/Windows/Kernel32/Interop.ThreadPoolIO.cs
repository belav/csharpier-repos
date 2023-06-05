// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        partial internal static unsafe SafeThreadPoolIOHandle CreateThreadpoolIo(
            SafeHandle fl,
            delegate* unmanaged<IntPtr, IntPtr, IntPtr, uint, UIntPtr, IntPtr, void> pfnio,
            IntPtr context,
            IntPtr pcbe
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe void CloseThreadpoolIo(IntPtr pio);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe void StartThreadpoolIo(SafeThreadPoolIOHandle pio);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe void CancelThreadpoolIo(SafeThreadPoolIOHandle pio);
    }
}
