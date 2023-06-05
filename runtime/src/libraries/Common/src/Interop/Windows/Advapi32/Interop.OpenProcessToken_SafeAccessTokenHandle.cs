// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(Interop.Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool OpenProcessToken(
            IntPtr ProcessToken,
            TokenAccessLevels DesiredAccess,
            out SafeAccessTokenHandle TokenHandle
        );
    }
}
