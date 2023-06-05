// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal unsafe class Sys
    {
        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_GetEnviron")]
        partial internal static unsafe IntPtr GetEnviron();

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_FreeEnviron")]
        partial internal static unsafe void FreeEnviron(IntPtr environ);
    }
}
