// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(Libraries.Advapi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool AllocateLocallyUniqueId(LUID* Luid);
    }
}
