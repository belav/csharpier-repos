// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Logoncli
    {
        [LibraryImport(
            Libraries.Logoncli,
            EntryPoint = "DsGetDcNameW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int DsGetDcName(
            string computerName,
            string domainName,
            IntPtr domainGuid,
            string siteName,
            int flags,
            out IntPtr domainControllerInfo
        );
    }
}
