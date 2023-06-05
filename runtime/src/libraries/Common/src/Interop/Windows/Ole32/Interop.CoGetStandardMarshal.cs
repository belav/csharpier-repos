// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Ole32
    {
        [LibraryImport(Interop.Libraries.Ole32)]
        partial internal static int CoGetStandardMarshal(
            ref Guid riid,
            IntPtr pv,
            int dwDestContext,
            IntPtr pvDestContext,
            int mshlflags,
            out IntPtr ppMarshal
        );
    }
}
