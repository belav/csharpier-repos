// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        partial internal static unsafe int CertVerifyTimeValidity(
            ref FILETIME pTimeToVerify,
            CERT_INFO* pCertInfo
        );
    }
}
