// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetNamedPipeHandleStateW(
            SafePipeHandle hNamedPipe,
            uint* lpState,
            uint* lpCurInstances,
            uint* lpMaxCollectionCount,
            uint* lpCollectDataTimeout,
            char* lpUserName,
            uint nMaxUserNameSize
        );
    }
}
