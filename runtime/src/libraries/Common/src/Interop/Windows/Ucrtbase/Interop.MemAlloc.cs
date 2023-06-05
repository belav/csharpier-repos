// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static unsafe class Ucrtbase
    {
#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
        [LibraryImport(Libraries.Ucrtbase)]
        [UnmanagedCallConv(
            CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) }
        )]
        partial internal static void* _aligned_malloc(nuint size, nuint alignment);

        [LibraryImport(Libraries.Ucrtbase)]
        [UnmanagedCallConv(
            CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) }
        )]
        partial internal static void _aligned_free(void* ptr);

        [LibraryImport(Libraries.Ucrtbase)]
        [UnmanagedCallConv(
            CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) }
        )]
        partial internal static void* _aligned_realloc(void* ptr, nuint size, nuint alignment);

        [LibraryImport(Libraries.Ucrtbase)]
        [UnmanagedCallConv(
            CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) }
        )]
        partial internal static void* calloc(nuint num, nuint size);

        [LibraryImport(Libraries.Ucrtbase)]
        [UnmanagedCallConv(
            CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) }
        )]
        partial internal static void free(void* ptr);

        [LibraryImport(Libraries.Ucrtbase)]
        [UnmanagedCallConv(
            CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) }
        )]
        partial internal static void* malloc(nuint size);

        [LibraryImport(Libraries.Ucrtbase)]
        [UnmanagedCallConv(
            CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) }
        )]
        partial internal static void* realloc(void* ptr, nuint new_size);
#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
    }
}
