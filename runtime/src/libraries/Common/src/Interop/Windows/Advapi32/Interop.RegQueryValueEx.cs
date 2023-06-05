// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if REGISTRY_ASSEMBLY
using Microsoft.Win32.SafeHandles;
#else
using Internal.Win32.SafeHandles;
#endif
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegQueryValueExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegQueryValueEx(
            SafeRegistryHandle hKey,
            string? lpValueName,
            int[]? lpReserved,
            ref int lpType,
            byte[]? lpData,
            ref int lpcbData
        );

        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegQueryValueExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int RegQueryValueEx(
            SafeRegistryHandle hKey,
            string? lpValueName,
            int* lpReserved,
            int* lpType,
            byte* lpData,
            uint* lpcbData
        );
    }
}
