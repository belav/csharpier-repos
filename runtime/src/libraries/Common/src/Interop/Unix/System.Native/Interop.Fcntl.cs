// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        partial internal static class Fcntl
        {
            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlSetIsNonBlocking",
                SetLastError = true
            )]
            partial internal static int DangerousSetIsNonBlocking(IntPtr fd, int isNonBlocking);

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlSetIsNonBlocking",
                SetLastError = true
            )]
            partial internal static int SetIsNonBlocking(SafeHandle fd, int isNonBlocking);

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlGetIsNonBlocking",
                SetLastError = true
            )]
            partial internal static int GetIsNonBlocking(
                SafeHandle fd,
                [MarshalAs(UnmanagedType.Bool)] out bool isNonBlocking
            );

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlSetFD",
                SetLastError = true
            )]
            partial internal static int SetFD(SafeHandle fd, int flags);

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlGetFD",
                SetLastError = true
            )]
            partial internal static int GetFD(SafeHandle fd);

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlGetFD",
                SetLastError = true
            )]
            partial internal static int GetFD(IntPtr fd);
        }
    }
}
