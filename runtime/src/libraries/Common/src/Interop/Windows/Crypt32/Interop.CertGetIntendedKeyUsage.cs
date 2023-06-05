// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        // Note: It's somewhat unusual to use an API enum as a parameter type to a P/Invoke but in this case, X509KeyUsageFlags was intentionally designed as bit-wise
        // identical to the wincrypt CERT_*_USAGE values.
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CertGetIntendedKeyUsage(
            CertEncodingType dwCertEncodingType,
            CERT_INFO* pCertInfo,
            out X509KeyUsageFlags pbKeyUsage,
            int cbKeyUsage
        );
    }
}
