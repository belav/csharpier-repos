// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [Flags]
        internal enum MemoryMappedSyncFlags
        {
            MS_ASYNC = 0x1,
            MS_SYNC = 0x2,
            MS_INVALIDATE = 0x10,
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_MSync",
            SetLastError = true
        )]
        partial internal static int MSync(IntPtr addr, ulong len, MemoryMappedSyncFlags flags);
    }
}
