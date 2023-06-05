// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        partial public static IntPtr GlobalFree(IntPtr handle);

        public static IntPtr GlobalFree(HandleRef handle)
        {
            IntPtr result = GlobalFree(handle.Handle);
            GC.KeepAlive(handle.Wrapper);
            return result;
        }
    }
}
