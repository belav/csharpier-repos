// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_InitializeTerminalAndSignalHandling",
            SetLastError = true
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool InitializeTerminalAndSignalHandling();

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_SetKeypadXmit",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static void SetKeypadXmit(string terminfoString);
    }
}
