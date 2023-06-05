// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Interop.Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CertFreeCertificateContext(IntPtr pCertContext);

        [LibraryImport(Interop.Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CertVerifyCertificateChainPolicy(
            IntPtr pszPolicyOID,
            SafeX509ChainHandle pChainContext,
            ref CERT_CHAIN_POLICY_PARA pPolicyPara,
            ref CERT_CHAIN_POLICY_STATUS pPolicyStatus
        );
    }
}
