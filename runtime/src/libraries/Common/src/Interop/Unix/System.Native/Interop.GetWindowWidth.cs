// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct WinSize
        {
            internal ushort Row;
            internal ushort Col;
            internal ushort XPixel;
            internal ushort YPixel;
        };

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_GetWindowSize",
            SetLastError = true
        )]
        partial internal static int GetWindowSize(out WinSize winSize);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_SetWindowSize",
            SetLastError = true
        )]
        partial internal static int SetWindowSize(in WinSize winSize);
    }
}
