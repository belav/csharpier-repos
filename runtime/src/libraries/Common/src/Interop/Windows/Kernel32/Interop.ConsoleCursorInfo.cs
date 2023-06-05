// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct CONSOLE_CURSOR_INFO
        {
            internal int dwSize;
            internal BOOL bVisible;
        }

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool GetConsoleCursorInfo(
            IntPtr hConsoleOutput,
            out CONSOLE_CURSOR_INFO cci
        );

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool SetConsoleCursorInfo(
            IntPtr hConsoleOutput,
            ref CONSOLE_CURSOR_INFO cci
        );
    }
}
