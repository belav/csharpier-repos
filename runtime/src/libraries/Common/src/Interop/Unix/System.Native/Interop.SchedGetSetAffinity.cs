// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_SchedSetAffinity",
            SetLastError = true
        )]
        partial internal static int SchedSetAffinity(int pid, ref IntPtr mask);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_SchedGetAffinity",
            SetLastError = true
        )]
        partial internal static int SchedGetAffinity(int pid, out IntPtr mask);
    }
}
