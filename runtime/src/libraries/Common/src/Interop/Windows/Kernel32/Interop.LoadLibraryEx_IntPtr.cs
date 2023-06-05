// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        internal const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        internal const int LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "LoadLibraryExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static IntPtr LoadLibraryEx(
            string libFilename,
            IntPtr reserved,
            int flags
        );
    }
}
