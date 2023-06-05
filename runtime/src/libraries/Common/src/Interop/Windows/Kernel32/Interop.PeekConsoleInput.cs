// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "PeekConsoleInputW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool PeekConsoleInput(
            IntPtr hConsoleInput,
            out INPUT_RECORD buffer,
            int numInputRecords_UseOne,
            out int numEventsRead
        );
    }
}
