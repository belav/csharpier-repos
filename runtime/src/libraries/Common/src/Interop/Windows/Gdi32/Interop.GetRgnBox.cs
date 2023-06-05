// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        partial public static RegionType GetRgnBox(IntPtr hrgn, ref RECT lprc);

        public static RegionType GetRgnBox(HandleRef hrgn, ref RECT lprc)
        {
            RegionType result = GetRgnBox(hrgn.Handle, ref lprc);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}
