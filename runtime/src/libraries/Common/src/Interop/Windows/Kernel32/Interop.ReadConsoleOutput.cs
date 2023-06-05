// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CHAR_INFO
        {
            private ushort charData;
            private short attributes;
        }

        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "ReadConsoleOutputW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool ReadConsoleOutput(
            IntPtr hConsoleOutput,
            CHAR_INFO* pBuffer,
            COORD bufferSize,
            COORD bufferCoord,
            ref SMALL_RECT readRegion
        );
    }
}
