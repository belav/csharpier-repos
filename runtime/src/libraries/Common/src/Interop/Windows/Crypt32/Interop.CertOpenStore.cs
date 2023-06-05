// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        internal const uint PKCS_7_ASN_ENCODING = 0x00010000;
        internal const uint X509_ASN_ENCODING = 0x00000001;
        internal const uint CERT_STORE_PROV_MEMORY = 2;

        [LibraryImport(Interop.Libraries.Crypt32, SetLastError = true)]
        partial internal static SafeCertStoreHandle CertOpenStore(
            IntPtr lpszStoreProvider,
            uint dwMsgAndCertEncodingType,
            IntPtr hCryptProv,
            uint dwFlags,
            IntPtr pvPara
        );
    }
}
