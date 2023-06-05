// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal unsafe class Sys
    {
        [LibraryImport(
            Interop.Libraries.SystemNative,
            EntryPoint = "SystemNative_LoadLibrary",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static IntPtr LoadLibrary(string filename);

        [LibraryImport(
            Interop.Libraries.SystemNative,
            EntryPoint = "SystemNative_GetLoadLibraryError"
        )]
        partial internal static IntPtr GetLoadLibraryError();

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_GetProcAddress")]
        partial internal static IntPtr GetProcAddress(IntPtr handle, byte* symbol);

        [LibraryImport(
            Interop.Libraries.SystemNative,
            EntryPoint = "SystemNative_GetProcAddress",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static IntPtr GetProcAddress(IntPtr handle, string symbol);

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_FreeLibrary")]
        partial internal static void FreeLibrary(IntPtr handle);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_GetDefaultSearchOrderPseudoHandle",
            SetLastError = true
        )]
        partial internal static IntPtr GetDefaultSearchOrderPseudoHandle();
    }
}
