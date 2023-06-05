// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        partial public static IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
    }
}
