// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, EntryPoint = "GlobalAlloc", SetLastError = true)]
        partial internal static IntPtr IntGlobalAlloc(int uFlags, UIntPtr dwBytes); // size should be 32/64bits compatible

        internal static IntPtr GlobalAlloc(int uFlags, uint dwBytes)
        {
            return IntGlobalAlloc(uFlags, new UIntPtr(dwBytes));
        }
    }
}
