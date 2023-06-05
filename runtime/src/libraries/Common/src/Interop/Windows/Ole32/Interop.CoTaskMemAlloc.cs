// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Ole32
    {
        [LibraryImport(Libraries.Ole32)]
        partial internal static IntPtr CoTaskMemAlloc(nuint cb);

        [LibraryImport(Libraries.Ole32)]
        partial internal static IntPtr CoTaskMemRealloc(IntPtr pv, nuint cb);

        [LibraryImport(Libraries.Ole32)]
        partial internal static void CoTaskMemFree(IntPtr ptr);
    }
}
