// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe IntPtr CreateThreadpoolWork(
            delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> pfnwk,
            IntPtr pv,
            IntPtr pcbe
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static void SubmitThreadpoolWork(IntPtr pwk);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static void CloseThreadpoolWork(IntPtr pwk);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe IntPtr CreateThreadpoolWait(
            delegate* unmanaged<IntPtr, IntPtr, IntPtr, uint, void> pfnwa,
            IntPtr pv,
            IntPtr pcbe
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static void SetThreadpoolWait(IntPtr pwa, IntPtr h, IntPtr pftTimeout);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static void WaitForThreadpoolWaitCallbacks(
            IntPtr pwa,
            [MarshalAs(UnmanagedType.Bool)] bool fCancelPendingCallbacks
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static void CloseThreadpoolWait(IntPtr pwa);
    }
}
