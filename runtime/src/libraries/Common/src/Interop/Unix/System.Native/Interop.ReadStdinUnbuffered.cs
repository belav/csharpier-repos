// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_ReadStdin",
            SetLastError = true
        )]
        partial internal static unsafe int ReadStdin(byte* buffer, int bufferSize);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_InitializeConsoleBeforeRead"
        )]
        partial internal static void InitializeConsoleBeforeRead(
            byte minChars = 1,
            byte decisecondsTimeout = 0
        );

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_UninitializeConsoleAfterRead"
        )]
        partial internal static void UninitializeConsoleAfterRead();
    }
}
