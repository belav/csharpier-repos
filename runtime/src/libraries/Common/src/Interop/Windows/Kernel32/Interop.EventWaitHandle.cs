// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        internal const uint CREATE_EVENT_INITIAL_SET = 0x2;
        internal const uint CREATE_EVENT_MANUAL_RESET = 0x1;

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool SetEvent(SafeWaitHandle handle);

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool ResetEvent(SafeWaitHandle handle);

        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "CreateEventExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static SafeWaitHandle CreateEventEx(
            IntPtr lpSecurityAttributes,
            string? name,
            uint flags,
            uint desiredAccess
        );

        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "OpenEventW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static SafeWaitHandle OpenEvent(
            uint desiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
            string name
        );
    }
}
