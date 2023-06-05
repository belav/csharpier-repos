// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial public static bool RestoreDC(IntPtr hdc, int nSavedDC);

        public static bool RestoreDC(HandleRef hdc, int nSavedDC)
        {
            bool result = RestoreDC(hdc.Handle, nSavedDC);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
