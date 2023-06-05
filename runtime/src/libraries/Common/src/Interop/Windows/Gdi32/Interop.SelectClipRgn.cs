// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(Libraries.Gdi32, SetLastError = true)]
        partial public static RegionType SelectClipRgn(IntPtr hdc, IntPtr hrgn);

        public static RegionType SelectClipRgn(HandleRef hdc, HandleRef hrgn)
        {
            RegionType result = SelectClipRgn(hdc.Handle, hrgn.Handle);
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}
