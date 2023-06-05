// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "OpenSemaphoreW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static SafeWaitHandle OpenSemaphore(
            uint desiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
            string name
        );

        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "CreateSemaphoreExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static SafeWaitHandle CreateSemaphoreEx(
            IntPtr lpSecurityAttributes,
            int initialCount,
            int maximumCount,
            string? name,
            uint flags,
            uint desiredAccess
        );

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool ReleaseSemaphore(
            SafeWaitHandle handle,
            int releaseCount,
            out int previousCount
        );
    }
}
