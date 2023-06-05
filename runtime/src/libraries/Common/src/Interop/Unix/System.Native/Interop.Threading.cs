// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal unsafe class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_CreateThread")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CreateThread(
            IntPtr stackSize,
            delegate* unmanaged<IntPtr, IntPtr> startAddress,
            IntPtr parameter
        );
    }
}
