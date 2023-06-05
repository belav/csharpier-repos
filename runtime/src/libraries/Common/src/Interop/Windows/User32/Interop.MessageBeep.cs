// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class User32
    {
        internal const int MB_OK = 0;
        internal const int MB_ICONHAND = 0x10;
        internal const int MB_ICONQUESTION = 0x20;
        internal const int MB_ICONEXCLAMATION = 0x30;
        internal const int MB_ICONASTERISK = 0x40;

        [LibraryImport(Libraries.User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool MessageBeep(int type);
    }
}
