// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static unsafe class Sys
    {
        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_AlignedAlloc")]
        partial internal static void* AlignedAlloc(nuint alignment, nuint size);

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_AlignedFree")]
        partial internal static void AlignedFree(void* ptr);

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_AlignedRealloc")]
        partial internal static void* AlignedRealloc(void* ptr, nuint alignment, nuint new_size);

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_Calloc")]
        partial internal static void* Calloc(nuint num, nuint size);

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_Free")]
        partial internal static void Free(void* ptr);

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_Malloc")]
        partial internal static void* Malloc(nuint size);

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_Realloc")]
        partial internal static void* Realloc(void* ptr, nuint new_size);
    }
}
