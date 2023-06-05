// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        partial internal static unsafe SafeCertContextHandle CertFindCertificateInStore(
            SafeCertStoreHandle hCertStore,
            CertEncodingType dwCertEncodingType,
            CertFindFlags dwFindFlags,
            CertFindType dwFindType,
            void* pvFindPara,
            CERT_CONTEXT* pPrevCertContext
        );
    }
}
