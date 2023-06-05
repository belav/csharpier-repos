// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(
            Interop.Libraries.Advapi32,
            EntryPoint = "GetNamedSecurityInfoW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static uint GetSecurityInfoByName(
            string name,
            /*DWORD*/uint objectType,
            /*DWORD*/uint securityInformation,
            out IntPtr sidOwner,
            out IntPtr sidGroup,
            out IntPtr dacl,
            out IntPtr sacl,
            out IntPtr securityDescriptor
        );
    }
}
