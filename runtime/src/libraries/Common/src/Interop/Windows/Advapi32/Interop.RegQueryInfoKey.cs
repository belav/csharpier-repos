// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if REGISTRY_ASSEMBLY
using Microsoft.Win32.SafeHandles;
#else
using Internal.Win32.SafeHandles;
#endif
using System;
using System.Runtime.InteropServices;
using System.Text;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegQueryInfoKeyW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegQueryInfoKey(
            SafeRegistryHandle hKey,
            [Out] char[]? lpClass,
            int[]? lpcbClass,
            IntPtr lpReserved_MustBeZero,
            ref int lpcSubKeys,
            int[]? lpcbMaxSubKeyLen,
            int[]? lpcbMaxClassLen,
            ref int lpcValues,
            int[]? lpcbMaxValueNameLen,
            int[]? lpcbMaxValueLen,
            int[]? lpcbSecurityDescriptor,
            int[]? lpftLastWriteTime
        );
    }
}
