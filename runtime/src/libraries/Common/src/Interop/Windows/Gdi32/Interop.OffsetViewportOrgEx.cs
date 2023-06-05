// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial public static bool OffsetViewportOrgEx(IntPtr hdc, int x, int y, ref Point lppt);

        public static bool OffsetViewportOrgEx(HandleRef hdc, int x, int y, ref Point lppt)
        {
            bool result = OffsetViewportOrgEx(hdc.Handle, x, y, ref lppt);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
