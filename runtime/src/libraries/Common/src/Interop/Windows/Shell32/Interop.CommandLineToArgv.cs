// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static unsafe class Interop
{
    partial internal static class Shell32
    {
        [LibraryImport(Libraries.Shell32, EntryPoint = "CommandLineToArgvW")]
        partial internal static char** CommandLineToArgv(char* lpCommandLine, int* pNumArgs);
    }
}
