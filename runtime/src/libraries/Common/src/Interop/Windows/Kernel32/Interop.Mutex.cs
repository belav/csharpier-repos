// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        internal const uint CREATE_MUTEX_INITIAL_OWNER = 0x1;

        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "OpenMutexW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static SafeWaitHandle OpenMutex(
            uint desiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
            string name
        );

        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "CreateMutexExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static SafeWaitHandle CreateMutexEx(
            IntPtr lpMutexAttributes,
            string? name,
            uint flags,
            uint desiredAccess
        );

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool ReleaseMutex(SafeWaitHandle handle);
    }
}
