// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(Interop.Libraries.Advapi32, EntryPoint = "GetSecurityInfo")]
        partial internal static unsafe uint GetSecurityInfoByHandle(
            SafeHandle handle,
            /*DWORD*/uint objectType,
            /*DWORD*/uint securityInformation,
            IntPtr* sidOwner,
            IntPtr* sidGroup,
            IntPtr* dacl,
            IntPtr* sacl,
            IntPtr* securityDescriptor
        );
    }
}
