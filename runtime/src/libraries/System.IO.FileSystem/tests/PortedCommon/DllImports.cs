// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Text;

partial internal static class DllImports
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    partial internal static int GetLogicalDrives();

    [LibraryImport(
        "kernel32.dll",
        EntryPoint = "GetDiskFreeSpaceExW",
        StringMarshalling = StringMarshalling.Utf16,
        SetLastError = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    partial internal static bool GetDiskFreeSpaceEx(
        string drive,
        out long freeBytesForUser,
        out long totalBytes,
        out long freeBytes
    );

    [LibraryImport(
        "kernel32.dll",
        EntryPoint = "GetDriveTypeW",
        StringMarshalling = StringMarshalling.Utf16,
        SetLastError = true
    )]
    partial internal static int GetDriveType(string drive);
}
