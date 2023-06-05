// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Threading;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        partial internal static unsafe int ReadFileScatter(
            SafeHandle hFile,
            long* aSegmentArray,
            int nNumberOfBytesToRead,
            IntPtr lpReserved,
            NativeOverlapped* lpOverlapped
        );

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        partial internal static unsafe int WriteFileGather(
            SafeHandle hFile,
            long* aSegmentArray,
            int nNumberOfBytesToWrite,
            IntPtr lpReserved,
            NativeOverlapped* lpOverlapped
        );
    }
}
