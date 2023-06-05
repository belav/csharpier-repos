// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        internal const uint CERT_STORE_ADD_ALWAYS = 4;

        [LibraryImport(Interop.Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CertAddCertificateLinkToStore(
            SafeCertStoreHandle hCertStore,
            SafeCertContextHandle pCertContext,
            uint dwAddDisposition,
            SafeCertContextHandle ppStoreContext
        );
    }
}
