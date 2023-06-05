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
        partial internal static unsafe bool GetNamedPipeInfo(
            SafePipeHandle hNamedPipe,
            uint* lpFlags,
            uint* lpOutBufferSize,
            uint* lpInBufferSize,
            uint* lpMaxInstances
        );
    }
}
