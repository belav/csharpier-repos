// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Threading;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetOverlappedResult(
            SafeFileHandle hFile,
            NativeOverlapped* lpOverlapped,
            ref int lpNumberOfBytesTransferred,
            [MarshalAs(UnmanagedType.Bool)] bool bWait
        );
    }
}
