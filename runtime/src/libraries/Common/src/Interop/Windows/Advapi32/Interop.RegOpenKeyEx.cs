// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if REGISTRY_ASSEMBLY
using Microsoft.Win32.SafeHandles;
#else
using Internal.Win32.SafeHandles;
#endif
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegOpenKeyExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegOpenKeyEx(
            SafeRegistryHandle hKey,
            string? lpSubKey,
            int ulOptions,
            int samDesired,
            out SafeRegistryHandle hkResult
        );

        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegOpenKeyExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegOpenKeyEx(
            IntPtr hKey,
            string? lpSubKey,
            int ulOptions,
            int samDesired,
            out SafeRegistryHandle hkResult
        );
    }
}
