// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal unsafe class Sys
    {
        [DoesNotReturn]
        [LibraryImport(Interop.Libraries.SystemNative, EntryPoint = "SystemNative_Exit")]
        partial internal static void Exit(int exitCode);
    }
}
