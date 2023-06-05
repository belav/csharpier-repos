// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe Interop.BOOL TzSpecificLocalTimeToSystemTime(
            IntPtr lpTimeZoneInformation,
            Interop.Kernel32.SYSTEMTIME* lpLocalTime,
            Interop.Kernel32.SYSTEMTIME* lpUniversalTime
        );
    }
}
