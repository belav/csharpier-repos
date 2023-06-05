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
            EntryPoint = "RegSetValueExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegSetValueEx(
            SafeRegistryHandle hKey,
            string? lpValueName,
            int Reserved,
            int dwType,
            byte[]? lpData,
            int cbData
        );

        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegSetValueExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegSetValueEx(
            SafeRegistryHandle hKey,
            string? lpValueName,
            int Reserved,
            int dwType,
            char[]? lpData,
            int cbData
        );

        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegSetValueExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegSetValueEx(
            SafeRegistryHandle hKey,
            string? lpValueName,
            int Reserved,
            int dwType,
            ref int lpData,
            int cbData
        );

        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegSetValueExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegSetValueEx(
            SafeRegistryHandle hKey,
            string? lpValueName,
            int Reserved,
            int dwType,
            ref long lpData,
            int cbData
        );

        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegSetValueExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int RegSetValueEx(
            SafeRegistryHandle hKey,
            string? lpValueName,
            int Reserved,
            int dwType,
            string? lpData,
            int cbData
        );
    }
}
