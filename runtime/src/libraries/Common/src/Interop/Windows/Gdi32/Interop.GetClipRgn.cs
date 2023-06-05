// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        partial public static int GetClipRgn(IntPtr hdc, IntPtr hrgn);

        public static int GetClipRgn(HandleRef hdc, IntPtr hrgn)
        {
            int result = GetClipRgn(hdc.Handle, hrgn);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }

        public static int GetClipRgn(HandleRef hdc, HandleRef hrgn)
        {
            int result = GetClipRgn(hdc.Handle, hrgn.Handle);
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}
