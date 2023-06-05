// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_CreateAutoreleasePool")]
        partial internal static IntPtr CreateAutoreleasePool();

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_DrainAutoreleasePool")]
        partial internal static void DrainAutoreleasePool(IntPtr ptr);
    }
}
