// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        // NOTE: The out parameters are PULARGE_INTEGERs and may require
        // some byte munging magic.
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "GetDiskFreeSpaceExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool GetDiskFreeSpaceEx(
            string drive,
            out long freeBytesForUser,
            out long totalBytes,
            out long freeBytes
        );
    }
}
