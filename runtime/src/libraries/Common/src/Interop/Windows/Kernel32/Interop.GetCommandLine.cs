// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static unsafe class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, EntryPoint = "GetCommandLineW")]
        partial internal static char* GetCommandLine();
    }
}
