// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(Interop.Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CheckTokenMembership(
            SafeAccessTokenHandle TokenHandle,
            byte[] SidToCheck,
            [MarshalAs(UnmanagedType.Bool)] ref bool IsMember
        );
    }
}
