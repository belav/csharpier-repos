// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal unsafe class Sys
    {
        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_GetErrNo")]
        [SuppressGCTransition]
        partial internal static int GetErrNo();

        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_SetErrNo")]
        [SuppressGCTransition]
        partial internal static void SetErrNo(int errorCode);
    }
}
