// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System;
using System.Security.Principal;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool OpenProcessToken(
            IntPtr ProcessToken,
            TokenAccessLevels DesiredAccess,
            out SafeTokenHandle TokenHandle
        );
    }
}
