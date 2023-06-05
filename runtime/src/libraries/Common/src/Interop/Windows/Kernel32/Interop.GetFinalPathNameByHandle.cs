// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        internal const uint FILE_NAME_NORMALIZED = 0x0;

        // https://docs.microsoft.com/windows/desktop/api/fileapi/nf-fileapi-getfinalpathnamebyhandlew (kernel32)
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "GetFinalPathNameByHandleW",
            SetLastError = true
        )]
        partial internal static unsafe uint GetFinalPathNameByHandle(
            SafeFileHandle hFile,
            char* lpszFilePath,
            uint cchFilePath,
            uint dwFlags
        );
    }
}
