// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static unsafe class Kernel32
    {
        [DoesNotReturn]
        [LibraryImport(Libraries.Kernel32, EntryPoint = "ExitProcess")]
        partial internal static void ExitProcess(int exitCode);
    }
}
