// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class SspiCli
    {
        [LibraryImport(Libraries.SspiCli)]
        partial internal static int LsaLookupAuthenticationPackage(
            SafeLsaHandle LsaHandle,
            ref Advapi32.LSA_STRING PackageName,
            out int AuthenticationPackage
        );
    }
}
