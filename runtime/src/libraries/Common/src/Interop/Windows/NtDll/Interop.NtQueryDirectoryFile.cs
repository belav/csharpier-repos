// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class NtDll
    {
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff556633.aspx
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff567047.aspx
        [LibraryImport(Libraries.NtDll)]
        partial public static unsafe int NtQueryDirectoryFile(
            IntPtr FileHandle,
            IntPtr Event,
            IntPtr ApcRoutine,
            IntPtr ApcContext,
            IO_STATUS_BLOCK* IoStatusBlock,
            IntPtr FileInformation,
            uint Length,
            FILE_INFORMATION_CLASS FileInformationClass,
            BOOLEAN ReturnSingleEntry,
            UNICODE_STRING* FileName,
            BOOLEAN RestartScan
        );
    }
}
