// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        // The actual native signature is:
        //      BOOL WINAPI QueryPerformanceCounter(
        //          _Out_ LARGE_INTEGER* lpPerformanceCount
        //      );
        //
        // We take a long* (rather than a out long) to avoid the pinning overhead.
        // We don't set last error since we don't need the extended error info.

        [LibraryImport(Libraries.Kernel32)]
        [SuppressGCTransition]
        partial internal static unsafe BOOL QueryPerformanceCounter(long* lpPerformanceCount);
    }
}
